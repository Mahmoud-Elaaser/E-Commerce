using ECommerce.DTOs.Product;
using FluentValidation;

namespace ECommerce.Validators.Product
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product ID must be greater than zero");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required")
                .MaximumLength(2000).WithMessage("Product description cannot exceed 2000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.ProductBrandId)
                .GreaterThan(0).WithMessage("Product brand ID must be greater than zero");

            RuleFor(x => x.ProductBrandName)
                .NotEmpty().WithMessage("Product brand name is required");

            RuleFor(x => x.ProductTypeId)
                .GreaterThan(0).WithMessage("Product type ID must be greater than zero");

            RuleFor(x => x.ProductTypeName)
                .NotEmpty().WithMessage("Product type name is required");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        }
    }
}