using AutoMapper;
using ECommerce.DTOs;
using ECommerce.DTOs.Pagination;
using ECommerce.DTOs.Product;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using ECommerce.Specifications;
using Product = ECommerce.Models.Product;
namespace ECommerce.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
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
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<ResponseDto> AddProductAsync(AddOrUpdateProductDto addProductDto)
        {
            try
            {
                var validationResult = await ValidateProductDtoAsync(addProductDto);
                if (validationResult != null)
                    return validationResult;

                var existingProduct = await CheckIfProductNameExistsAsync(addProductDto.Name);
                if (existingProduct)
                    return ResponseDto.Failure(400, $"Product '{addProductDto.Name}' already exists");

                string? imageUrl = null;
                if (addProductDto is AddOrUpdateProductDto dtoWithImage &&
                    dtoWithImage.ImageFile != null)
                {
                    if (!_fileService.IsValidImage(dtoWithImage.ImageFile))
                    {
                        return ResponseDto.Failure(400, "Invalid image file. Please upload a valid image (JPG, PNG, GIF, BMP, WEBP) under 5MB.");
                    }

                    var uploadResult = await _fileService.UploadFileAsync(dtoWithImage.ImageFile, "products");

                    if (!uploadResult.Success)
                    {
                        return ResponseDto.Failure(500, $"Failed to upload image: {uploadResult.ErrorMessage}");
                    }

                    imageUrl = uploadResult.FileUrl;
                }

                var product = _mapper.Map<Product>(addProductDto);
                product.ImageUrl = imageUrl;
                await _unitOfWork.Repository<Product>().AddAsync(product);
                await _unitOfWork.CompleteAsync();

                var productDto = _mapper.Map<ProductDto>(product);

                return ResponseDto.Success(201, "Product created successfully", productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return ResponseDto.Failure(500, "An error occurred while creating the product");
            }
        }

        public async Task<ResponseDto> DeleteProductAsync(int productId)
        {
            try
            {
                var existingProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (existingProduct == null)
                    return ResponseDto.Failure(404, $"Product with ID {productId} not found");

                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var deleted = await _fileService.DeleteFileAsync(existingProduct.ImageUrl);
                    if (!deleted)
                    {
                        _logger.LogWarning("Failed to delete image for product {ProductId}: {ImageUrl}",
                            productId, existingProduct.ImageUrl);
                    }
                }

                _unitOfWork.Repository<Product>().Delete(existingProduct);
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

                var products = await _unitOfWork.Repository<Product>().ListAsync(spec);
                var totalCount = await _unitOfWork.Repository<Product>().CountAsync(spec);

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

                var metadata = new PaginationMetadata(totalCount, paginationParams.PageNumber, paginationParams.PageSize);
                var paginationResult = new PaginationResponse<ProductDto>(productDtos, metadata);

                return ResponseDto.Success(200, "Products retrieved successfully", paginationResult);
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
                var product = await _unitOfWork.Repository<Product>().GetAsyncWithSpec(spec);
                if (product == null)
                    return ResponseDto.Failure(404, $"Product with ID {productId} not found");

                var productDto = _mapper.Map<ProductDto>(product);

                return ResponseDto.Success(200, "Product retrieved successfully", productDto);
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
                var brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(brandId);
                if (brand == null)
                    return ResponseDto.Failure(404, $"Product brand with ID {brandId} not found");

                var spec = new ProductSpecification(
                    searchTerm: paginationParams.SearchTerm,
                    brandId: brandId,
                    includeBrand: true,
                    includeType: true);

                spec.ApplySorting(paginationParams.SortBy, paginationParams.SortDescending);
                spec.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

                var products = await _unitOfWork.Repository<Product>().ListAsync(spec);
                var totalCount = await _unitOfWork.Repository<Product>().CountAsync(spec);

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

                var metadata = new PaginationMetadata(totalCount, paginationParams.PageNumber, paginationParams.PageSize);
                var paginationResult = new PaginationResponse<ProductDto>(productDtos, metadata);

                return ResponseDto.Success(200, "Products retrieved successfully", paginationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products by brand ID: {BrandId}", brandId);
                return ResponseDto.Failure(500, "An error occurred while retrieving products");
            }
        }

        public async Task<ResponseDto> GetProductsByTypeAsync(int typeId, PaginationParams paginationParams)
        {
            try
            {
                var type = await _unitOfWork.Repository<ProductType>().GetByIdAsync(typeId);
                if (type == null)
                    return ResponseDto.Failure(404, $"Product type with ID {typeId} not found");

                var spec = new ProductSpecification(
                    searchTerm: paginationParams.SearchTerm,
                    typeId: typeId,
                    includeBrand: true,
                    includeType: true);

                spec.ApplySorting(paginationParams.SortBy, paginationParams.SortDescending);
                spec.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

                var products = await _unitOfWork.Repository<Product>().ListAsync(spec);
                var totalCount = await _unitOfWork.Repository<Product>().CountAsync(spec);

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

                var metadata = new PaginationMetadata(totalCount, paginationParams.PageNumber, paginationParams.PageSize);
                var paginationResult = new PaginationResponse<ProductDto>(productDtos, metadata);

                return ResponseDto.Success(200, "Products retrieved successfully", paginationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products by type ID: {TypeId}", typeId);
                return ResponseDto.Failure(500, "An error occurred while retrieving products");
            }
        }

        public async Task<ResponseDto> UpdateProductAsync(int productId, AddOrUpdateProductDto updateProductDto)
        {
            try
            {
                var validationResult = await ValidateProductDtoAsync(updateProductDto);
                if (validationResult != null)
                    return validationResult;

                var existingProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (existingProduct == null)
                    return ResponseDto.Failure(404, $"Product with ID {productId} not found");

                var nameExists = await CheckIfProductNameExistsAsync(updateProductDto.Name, productId);
                if (nameExists)
                    return ResponseDto.Failure(400, $"Another product with name '{updateProductDto.Name}' already exists");


                if (updateProductDto.ImageFile != null)
                {
                    if (!_fileService.IsValidImage(updateProductDto.ImageFile))
                    {
                        return ResponseDto.Failure(400, "Invalid image file. Please upload a valid image (JPG, PNG, GIF, BMP, WEBP) under 5MB.");
                    }

                    var uploadResult = await _fileService.UpdateFileAsync(
                        updateProductDto.ImageFile,
                        existingProduct.ImageUrl,
                        "products");

                    if (!uploadResult.Success)
                    {
                        return ResponseDto.Failure(500, $"Failed to update image: {uploadResult.ErrorMessage}");
                    }

                    existingProduct.ImageUrl = uploadResult.FileUrl;
                }


                _mapper.Map(updateProductDto, existingProduct);
                existingProduct.Id = productId;

                _unitOfWork.Repository<Product>().Update(existingProduct);
                await _unitOfWork.CompleteAsync();

                var productDto = _mapper.Map<ProductDto>(existingProduct);

                return ResponseDto.Success(200, "Product updated successfully", productDto);
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
                    return ResponseDto.Failure(400, "Search term cannot be empty");

                var spec = new ProductSpecification(
                    searchTerm: searchTerm,
                    includeBrand: true,
                    includeType: true);

                spec.ApplySorting(paginationParams.SortBy, paginationParams.SortDescending);
                spec.ApplyPagination(paginationParams.PageNumber, paginationParams.PageSize);

                var products = await _unitOfWork.Repository<Product>().ListAsync(spec);
                var totalCount = await _unitOfWork.Repository<Product>().CountAsync(spec);

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

                var metadata = new PaginationMetadata(totalCount, paginationParams.PageNumber, paginationParams.PageSize);
                var paginationResult = new PaginationResponse<ProductDto>(productDtos, metadata);

                return ResponseDto.Success(200, "Products retrieved successfully", paginationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term: {SearchTerm}", searchTerm);
                return ResponseDto.Failure(500, "An error occurred while searching products");
            }
        }



        private async Task<bool> CheckIfProductNameExistsAsync(string productName, int? excludeProductId = null)
        {
            var allProducts = await _unitOfWork.Repository<Product>().ListAllAsync();

            if (excludeProductId.HasValue)
            {
                return allProducts.Any(p =>
                    p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase) &&
                    p.Id != excludeProductId.Value);
            }

            return allProducts.Any(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<ResponseDto> ValidateProductDtoAsync(AddOrUpdateProductDto dto)
        {
            if (dto == null)
                return ResponseDto.Failure(400, "Please enter valid data");

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Product name is required");
            else if (dto.Name.Length > 200)
                errors.Add("Product name cannot exceed 200 characters");

            if (dto.Description != null && dto.Description.Length > 1000)
                errors.Add("Product description cannot exceed 1000 characters");

            if (dto.Price <= 0)
                errors.Add("Product price must be greater than zero");

            if (dto.ProductBrandId <= 0)
            {
                errors.Add("Valid product brand is required");
            }
            else
            {
                var brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(dto.ProductBrandId);
                if (brand == null)
                    errors.Add($"Product brand with ID {dto.ProductBrandId} does not exist");
            }

            if (dto.ProductTypeId <= 0)
            {
                errors.Add("Valid product type is required");
            }
            else
            {
                var type = await _unitOfWork.Repository<ProductType>().GetByIdAsync(dto.ProductTypeId);
                if (type == null)
                    errors.Add($"Product type with ID {dto.ProductTypeId} does not exist");
            }

            if (errors.Any())
                return ResponseDto.ValidationFailure(400, "Validation failed", errors);

            return null;
        }
    }
}