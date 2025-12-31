using ECommerce.DTOs.Product;
using FluentValidation;

namespace ECommerce.Validators.Product
{
    public class ProductTypeDtoValidator : AbstractValidator<ProductTypeDto>
    {
        public ProductTypeDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Type ID must be greater than zero");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Type name is required")
                .MaximumLength(100).WithMessage("Type name cannot exceed 100 characters");
        }
    }
}