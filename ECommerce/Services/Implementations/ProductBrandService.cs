using AutoMapper;
using ECommerce.DTOs;
using ECommerce.DTOs.Product;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;

namespace ECommerce.Services.Implementations
{
    public class ProductBrandService : IProductBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductBrandService> _logger;

        public ProductBrandService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductBrandService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto> CreateBrandAsync(CreateOrUpdateBrandDto dto)
        {
            try
            {
                var existingBrand = await CheckIfBrandNameExistsAsync(dto.Name);
                if (existingBrand)
                    return ResponseDto.Failure(400, $"Product brand '{dto.Name}' already exists");

                var productBrand = _mapper.Map<ProductBrand>(dto);
                await _unitOfWork.Repository<ProductBrand>().AddAsync(productBrand);
                await _unitOfWork.CompleteAsync();

                var brandDto = _mapper.Map<ProductBrandDto>(productBrand);

                return ResponseDto.Success(201, "Product brand created successfully", brandDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product brand");
                return ResponseDto.Failure(500, "An error occurred while creating the product brand");
            }
        }

        public async Task<ResponseDto> DeleteBrandAsync(int id)
        {
            try
            {
                var existingBrand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);
                if (existingBrand == null)
                    return ResponseDto.Failure(404, $"Product brand with ID {id} not found");

                /// Check if brand has associated products
                var productCount = await CountProductsByBrandAsync(id);
                if (productCount > 0)
                    return ResponseDto.Failure(400,
                        $"Cannot delete product brand. There are {productCount} product(s) associated with this brand.");

                _unitOfWork.Repository<ProductBrand>().Delete(existingBrand);
                await _unitOfWork.CompleteAsync();

                return ResponseDto.Success(200, "Product brand deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product brand with ID: {BrandId}", id);
                return ResponseDto.Failure(500, "An error occurred while deleting the product brand");
            }
        }

        public async Task<ResponseDto> GetAllBrandsAsync()
        {

            var brands = await _unitOfWork.Repository<ProductBrand>().ListAllAsync();

            var brandDtos = _mapper.Map<IEnumerable<ProductBrandDto>>(brands);

            return ResponseDto.Success(200, "Product brands retrieved successfully", brandDtos);
        }

        public async Task<ResponseDto> GetBrandByIdAsync(int id)
        {

            var brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);
            if (brand == null)
                return ResponseDto.Failure(404, $"Product brand with ID {id} not found");

            var mappedBrand = _mapper.Map<ProductBrandDto>(brand);

            return ResponseDto.Success(200, "Product brand retrieved successfully", mappedBrand);
        }

        public async Task<bool> BrandExistsAsync(int id)
        {
            var brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);
            return brand != null;
        }

        public async Task<ResponseDto> UpdateBrandAsync(int id, CreateOrUpdateBrandDto dto)
        {
            try
            {
                var existingBrand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);
                if (existingBrand == null)
                    return ResponseDto.Failure(404, $"Product brand with ID {id} not found");

                var nameExists = await CheckIfBrandNameExistsAsync(dto.Name, id);
                if (nameExists)
                    return ResponseDto.Failure(400, $"Another product brand with name '{dto.Name}' already exists");

                _mapper.Map(dto, existingBrand);
                existingBrand.Id = id;

                _unitOfWork.Repository<ProductBrand>().Update(existingBrand);
                await _unitOfWork.CompleteAsync();

                var brandDto = _mapper.Map<ProductBrandDto>(existingBrand);

                return ResponseDto.Success(200, "Product brand updated successfully", brandDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product brand with ID: {BrandId}", id);
                return ResponseDto.Failure(500, "An error occurred while updating the product brand");
            }
        }

        private async Task<bool> CheckIfBrandNameExistsAsync(string brandName, int? excludeBrandId = null)
        {
            var allBrands = await _unitOfWork.Repository<ProductBrand>().ListAllAsync();

            if (excludeBrandId.HasValue)
            {
                return allBrands.Any(b =>
                    b.Name.Equals(brandName, StringComparison.OrdinalIgnoreCase) &&
                    b.Id != excludeBrandId.Value);
            }

            return allBrands.Any(b => b.Name.Equals(brandName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<int> CountProductsByBrandAsync(int brandId)
        {
            var allProducts = await _unitOfWork.Repository<Product>().ListAllAsync();
            return allProducts.Count(p => p.ProductBrandId == brandId);
        }

    }
}