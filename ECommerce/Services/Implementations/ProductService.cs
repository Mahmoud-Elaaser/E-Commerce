using AutoMapper;
using ECommerce.DTOs;
using ECommerce.DTOs.Pagination;
using ECommerce.DTOs.Product;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Specifications;

namespace ECommerce.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductBrand> _brandRepository;
        private readonly IGenericRepository<ProductType> _typeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IFileService _fileService;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductService> logger,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _productRepository = unitOfWork.Repository<Product>();
            _brandRepository = unitOfWork.Repository<ProductBrand>();
            _typeRepository = unitOfWork.Repository<ProductType>();
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<ResponseDto> AddProductAsync(AddOrUpdateProductDto addProductDto)
        {
            try
            {
                var validationResult = ValidationHelper.ValidateProductDto(addProductDto);
                if (validationResult != null)
                    return validationResult;

                var brand = await _brandRepository.GetByIdAsync(addProductDto.ProductBrandId);
                if (brand == null)
                    return ResponseDto.Failure(400, $"Brand with ID {addProductDto.ProductBrandId} does not exist");

                var type = await _typeRepository.GetByIdAsync(addProductDto.ProductTypeId);
                if (type == null)
                    return ResponseDto.Failure(400, $"Type with ID {addProductDto.ProductTypeId} does not exist");
                //-----------------------------------------------------
                string pictureUrl = null;
                if (addProductDto.PictureUrl != null)
                {
                    var uploadResult = await _fileService.UploadFileAsync(addProductDto.PictureUrl, "products");

                    if (!uploadResult.Success)
                        return ResponseDto.Failure(400, $"Failed to upload image: {uploadResult.ErrorMessage}");

                    pictureUrl = uploadResult.FileUrl;
                }
                //-----------------------------------------------------
                var product = _mapper.Map<Product>(addProductDto);
                await _productRepository.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                return ResponseDto.Success(201, "Product added successfully", new { ProductId = product.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                return ResponseDto.Failure(500, "An error occurred while adding the product");
            }
        }

        public async Task<ResponseDto> DeleteProductAsync(int productId)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(productId);
                if (existingProduct == null)
                    return ResponseDto.Failure(404, $"Product with ID {productId} not found");
                //-----------------------------------------------------
                if (!string.IsNullOrEmpty(existingProduct.PictureUrl))
                {
                    var deleteSuccess = await _fileService.DeleteFileAsync(existingProduct.PictureUrl);
                    if (!deleteSuccess)
                    {
                        _logger.LogWarning("Failed to delete image file for product ID: {ProductId}", productId);
                    }
                }
                //-----------------------------------------------------
                _productRepository.Delete(existingProduct);
                await _unitOfWork.CompleteAsync();

                return ResponseDto.Success(200, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", productId);
                return ResponseDto.Failure(500, "An error occurred while deleting the product");
            }
        }

        public async Task<ResponseDto> GetAllProductsAsync(ProductPaginationParams paginationParams)
        {
            try
            {
                var spec = new ProductSpecification(
                    searchTerm: paginationParams.SearchTerm,
                    brandId: paginationParams.BrandId,
                    typeId: paginationParams.TypeId,
                    minPrice: paginationParams.MinPrice,
                    maxPrice: paginationParams.MaxPrice,
                    inStockOnly: paginationParams.InStockOnly,
                    includeBrand: paginationParams.IncludeBrand,
                    includeType: paginationParams.IncludeType);

                spec.ApplySorting(paginationParams.SortBy, paginationParams.SortDescending);

                spec.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

                var countSpec = new ProductSpecification(
                    searchTerm: paginationParams.SearchTerm,
                    brandId: paginationParams.BrandId,
                    typeId: paginationParams.TypeId,
                    minPrice: paginationParams.MinPrice,
                    maxPrice: paginationParams.MaxPrice,
                    inStockOnly: paginationParams.InStockOnly);

                var totalCount = await _productRepository.CountAsync(countSpec);

                var products = await _productRepository.ListAsync(spec);
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

                return ResponseDto.PaginatedSuccess(
                    productDtos,
                    totalCount,
                    paginationParams.PageNumber,
                    paginationParams.PageSize,
                    "Products retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                return ResponseDto.Failure(500, "An error occurred while retrieving products");
            }
        }

        public async Task<ResponseDto> GetProductByIdAsync(int productId, bool includeBrand = false, bool includeType = false)
        {
            try
            {
                var spec = new ProductSpecification(productId, includeBrand, includeType);
                var product = (await _productRepository.ListAsync(spec)).FirstOrDefault();

                if (product == null)
                    return ResponseDto.Failure(404, $"Product with ID {productId} not found");

                var productDto = _mapper.Map<ProductDto>(product);

                return ResponseDto.Success(200,
                    "Product retrieved successfully",
                    productDto
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", productId);
                return ResponseDto.Failure(500, "An error occurred while retrieving the product");
            }
        }

        public async Task<ResponseDto> GetProductsByBrandAsync(int brandId, PaginationParams paginationParams)
        {
            try
            {
                if (brandId <= 0)
                    return ResponseDto.Failure(400, "Invalid brand ID");

                var brand = await _brandRepository.GetByIdAsync(brandId);
                if (brand == null)
                    return ResponseDto.Failure(404, $"Brand with ID {brandId} not found");

                var productParams = new ProductPaginationParams
                {
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize,
                    SearchTerm = paginationParams.SearchTerm,
                    SortBy = paginationParams.SortBy,
                    SortDescending = paginationParams.SortDescending,
                    BrandId = brandId
                };

                return await GetAllProductsAsync(productParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products by brand ID: {BrandId}", brandId);
                return ResponseDto.Failure(500, "An error occurred while retrieving products by brand");
            }
        }

        public async Task<ResponseDto> GetProductsByTypeAsync(int typeId, PaginationParams paginationParams)
        {
            try
            {
                if (typeId <= 0)
                    return ResponseDto.Failure(400, "Invalid type ID");

                var type = await _typeRepository.GetByIdAsync(typeId);
                if (type == null)
                    return ResponseDto.Failure(404, $"Type with ID {typeId} not found");

                var productParams = new ProductPaginationParams
                {
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize,
                    SearchTerm = paginationParams.SearchTerm,
                    SortBy = paginationParams.SortBy,
                    SortDescending = paginationParams.SortDescending,
                    TypeId = typeId
                };

                return await GetAllProductsAsync(productParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products by type ID: {TypeId}", typeId);
                return ResponseDto.Failure(500, "An error occurred while retrieving products by type");
            }
        }

        public async Task<ResponseDto> UpdateProductAsync(int productId, AddOrUpdateProductDto updateProductDto)
        {
            try
            {
                var validationResult = ValidationHelper.ValidateProductDto(updateProductDto);
                if (validationResult != null)
                    return validationResult;

                var brand = await _brandRepository.GetByIdAsync(updateProductDto.ProductBrandId);
                if (brand == null)
                    return ResponseDto.Failure(400, $"Brand with ID {updateProductDto.ProductBrandId} does not exist");


                var type = await _typeRepository.GetByIdAsync(updateProductDto.ProductTypeId);
                if (type == null)
                    return ResponseDto.Failure(400, $"Type with ID {updateProductDto.ProductTypeId} does not exist");


                var existingProduct = await _productRepository.GetByIdAsync(productId);
                if (existingProduct == null)
                    return ResponseDto.Failure(404, $"Product with ID {productId} not found");
                //-----------------------------------------------------
                string pictureUrl = existingProduct.PictureUrl;
                if (updateProductDto.PictureUrl != null)
                {
                    var updateResult = await _fileService.UpdateFileAsync(
                        updateProductDto.PictureUrl,
                        existingProduct.PictureUrl,
                        "products");

                    if (!updateResult.Success)
                        return ResponseDto.Failure(400, $"Failed to update image: {updateResult.ErrorMessage}");

                    pictureUrl = updateResult.FileUrl;
                }
                //-----------------------------------------------------

                _mapper.Map(updateProductDto, existingProduct);
                existingProduct.Id = productId; // Ensure ID remains unchanged

                _productRepository.Update(existingProduct);
                await _unitOfWork.CompleteAsync();

                return ResponseDto.Success(200, "Product updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", productId);
                return ResponseDto.Failure(500, "An error occurred while updating the product");
            }
        }

        public async Task<ResponseDto> SearchProductsAsync(string searchTerm, PaginationParams paginationParams)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return ResponseDto.Failure(400, "Search term is required");

                var productParams = new ProductPaginationParams
                {
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize,
                    SearchTerm = searchTerm,
                    SortBy = paginationParams.SortBy,
                    SortDescending = paginationParams.SortDescending
                };

                return await GetAllProductsAsync(productParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
                return ResponseDto.Failure(500, "An error occurred while searching products");
            }
        }

        public async Task<ResponseDto> UpdateProductStockAsync(int productId, int quantity)
        {
            try
            {
                if (quantity < 0)
                    return ResponseDto.Failure(400, "Quantity cannot be negative");

                var existingProduct = await _productRepository.GetByIdAsync(productId);
                if (existingProduct == null)
                    return ResponseDto.Failure(404, $"Product with ID {productId} not found");

                existingProduct.QuantityInStock = quantity;
                _productRepository.Update(existingProduct);
                await _unitOfWork.CompleteAsync();

                return ResponseDto.Success(200, "Product stock updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product ID: {ProductId}", productId);
                return ResponseDto.Failure(500, "An error occurred while updating product stock");
            }
        }
    }
}