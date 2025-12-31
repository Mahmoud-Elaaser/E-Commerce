using FluentValidation;
using ECommerce.DTOs.Product;

namespace ECommerce.Validators.Product
{
    public class CreateOrUpdateBrandDtoValidator : AbstractValidator<CreateOrUpdateBrandDto>
    {
        public CreateOrUpdateBrandDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand name is required")
                .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-&.]+$").WithMessage("Brand name contains invalid characters");
        }
    }
}