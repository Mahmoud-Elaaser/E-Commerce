using AutoMapper;
using ECommerce.DTOs;
using ECommerce.DTOs.Product;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;

namespace ECommerce.Services.Implementations
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductTypeService> _logger;

        public ProductTypeService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto> CreateTypeAsync(CreateOrUpdateTypeDto dto)
        {
            try
            {
                var validationResult = ValidateTypeDto(dto);
                if (validationResult != null)
                    return validationResult;

                var existingType = await CheckIfTypeNameExistsAsync(dto.Name);
                if (existingType)
                    return ResponseDto.Failure(400, $"Product type '{dto.Name}' already exists");

                var productType = _mapper.Map<ProductType>(dto);
                await _unitOfWork.Repository<ProductType>().AddAsync(productType);
                await _unitOfWork.CompleteAsync();

                var typeDto = _mapper.Map<ProductTypeDto>(productType);

                return ResponseDto.Success(201, "Product type created successfully", typeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product type");
                return ResponseDto.Failure(500, "An error occurred while creating the product type");
            }
        }

        public async Task<ResponseDto> DeleteTypeAsync(int id)
        {
            try
            {
                var existingType = await _unitOfWork.Repository<ProductType>().GetByIdAsync(id);
                if (existingType == null)
                    return ResponseDto.Failure(404, $"Product type with ID {id} not found");

                /// Check if type has associated products
                var productCount = await CountProductsByTypeAsync(id);
                if (productCount > 0)
                    return ResponseDto.Failure(400,
                        $"Cannot delete product type. There are {productCount} product(s) associated with this type.");

                _unitOfWork.Repository<ProductType>().Delete(existingType);
                await _unitOfWork.CompleteAsync();

                return ResponseDto.Success(200, "Product type deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product type with ID: {TypeId}", id);
                return ResponseDto.Failure(500, "An error occurred while deleting the product type");
            }
        }

        public async Task<ResponseDto> GetAllTypesAsync()
        {
            try
            {
                var types = await _unitOfWork.Repository<ProductType>().ListAllAsync();

                var typeDtos = _mapper.Map<IEnumerable<ProductTypeDto>>(types);

                return ResponseDto.Success(200, "Product types retrieved successfully", typeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all product types");
                return ResponseDto.Failure(500, "An error occurred while retrieving product types");
            }
        }

        public async Task<ResponseDto> GetTypeByIdAsync(int id)
        {
            try
            {
                var type = await _unitOfWork.Repository<ProductType>().GetByIdAsync(id);
                if (type == null)
                    return ResponseDto.Failure(404, $"Product type with ID {id} not found");

                var typeDto = _mapper.Map<ProductTypeDto>(type);

                return ResponseDto.Success(200, "Product type retrieved successfully", typeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product type with ID: {TypeId}", id);
                return ResponseDto.Failure(500, "An error occurred while retrieving the product type");
            }
        }

        public async Task<bool> TypeExistsAsync(int id)
        {
            try
            {
                var type = await _unitOfWork.Repository<ProductType>().GetByIdAsync(id);
                return type != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if type exists with ID: {TypeId}", id);
                return false;
            }
        }

        public async Task<ResponseDto> UpdateTypeAsync(int id, CreateOrUpdateTypeDto dto)
        {
            try
            {
                var validationResult = ValidateTypeDto(dto);
                if (validationResult != null)
                    return validationResult;

                var existingType = await _unitOfWork.Repository<ProductType>().GetByIdAsync(id);
                if (existingType == null)
                    return ResponseDto.Failure(404, $"Product type with ID {id} not found");

                var nameExists = await CheckIfTypeNameExistsAsync(dto.Name, id);
                if (nameExists)
                    return ResponseDto.Failure(400, $"Another product type with name '{dto.Name}' already exists");

                _mapper.Map(dto, existingType);
                existingType.Id = id;

                _unitOfWork.Repository<ProductType>().Update(existingType);
                await _unitOfWork.CompleteAsync();

                var typeDto = _mapper.Map<ProductTypeDto>(existingType);

                return ResponseDto.Success(200, "Product type updated successfully", typeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product type with ID: {TypeId}", id);
                return ResponseDto.Failure(500, "An error occurred while updating the product type");
            }
        }



        private async Task<bool> CheckIfTypeNameExistsAsync(string typeName, int? excludeTypeId = null)
        {
            var allTypes = await _unitOfWork.Repository<ProductType>().ListAllAsync();

            if (excludeTypeId.HasValue)
            {
                return allTypes.Any(t =>
                    t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) &&
                    t.Id != excludeTypeId.Value);
            }

            return allTypes.Any(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<int> CountProductsByTypeAsync(int typeId)
        {
            var allProducts = await _unitOfWork.Repository<Product>().ListAllAsync();
            return allProducts.Count(p => p.ProductTypeId == typeId);
        }



        private ResponseDto ValidateTypeDto(CreateOrUpdateTypeDto dto)
        {
            if (dto == null)
                return ResponseDto.Failure(400, "Please enter valid data");

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Product type name is required");
            else if (dto.Name.Length > 100)
                errors.Add("Product type name cannot exceed 100 characters");

            if (errors.Any())
                return ResponseDto.ValidationFailure(400, "Validation failed", errors);

            return null;
        }


    }
}