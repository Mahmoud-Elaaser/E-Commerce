using ECommerce.DTOs;
using ECommerce.DTOs.Product;

namespace ECommerce.Helpers
{
    public static class ValidationHelper
    {
        public static ResponseDto ValidateProductDto(AddOrUpdateProductDto dto)
        {
            if (dto == null)
                return ResponseDto.Failure(400, "Please enter valid data");

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Product name is required");
            else if (dto.Name.Length > 200)
                errors.Add("Product name cannot exceed 200 characters");

            if (dto.Price <= 0)
                errors.Add("Product price must be greater than zero");
            else if (dto.Price > 1000000)
                errors.Add("Product price cannot exceed 1,000,000");

            if (dto.QuantityInStock < 0)
                errors.Add("Quantity cannot be negative");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Product description is required");
            else if (dto.Description.Length > 1000)
                errors.Add("Description cannot exceed 1000 characters");

            if (dto.ProductBrandId <= 0)
                errors.Add("Brand ID must be greater than 0");

            if (dto.ProductTypeId <= 0)
                errors.Add("Type ID must be greater than 0");

            if (dto.PictureUrl == null)
                errors.Add("Picture URL cannot be null");

            if (dto.PictureUrl != null && dto.PictureUrl.Length > 500)
                errors.Add("Picture URL cannot exceed 500 characters");

            if (errors.Any())
                return ResponseDto.ValidationFailure(400, "Validation failed", errors);

            return null; // No validation errors
        }
    }
}
