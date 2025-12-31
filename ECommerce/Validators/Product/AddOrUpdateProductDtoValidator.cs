using ECommerce.DTOs.Product;
using FluentValidation;

namespace ECommerce.Validators.Product
{
    public class AddOrUpdateProductDtoValidator : AbstractValidator<AddOrUpdateProductDto>
    {
        public AddOrUpdateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required")
                .MaximumLength(2000).WithMessage("Product description cannot exceed 2000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero")
                .LessThanOrEqualTo(999999.99m).WithMessage("Price cannot exceed 999,999.99");

            RuleFor(x => x.ProductBrandId)
                .GreaterThan(0).WithMessage("Product brand must be selected");

            RuleFor(x => x.ProductTypeId)
                .GreaterThan(0).WithMessage("Product type must be selected");

            RuleFor(x => x.ImageFile)
                .Must(BeAValidImage).WithMessage("Image must be a valid image file (jpg, jpeg, png, gif)")
                .Must(BeWithinSizeLimit).WithMessage("Image size must not exceed 5MB")
                .When(x => x.ImageFile != null);
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private bool BeWithinSizeLimit(IFormFile? file)
        {
            if (file == null) return true;
            return file.Length <= 5 * 1024 * 1024; // 5MB
        }
    }
}