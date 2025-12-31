using ECommerce.DTOs.Product;
using FluentValidation;

namespace ECommerce.Validators.Product
{
    public class ProductBrandDtoValidator : AbstractValidator<ProductBrandDto>
    {
        public ProductBrandDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Brand ID must be greater than zero");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required")
                .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters");
        }
    }
}