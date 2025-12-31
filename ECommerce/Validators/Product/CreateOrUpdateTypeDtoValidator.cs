using ECommerce.DTOs.Product;
using FluentValidation;

namespace ECommerce.Validators.Product
{
    public class CreateOrUpdateTypeDtoValidator : AbstractValidator<CreateOrUpdateTypeDto>
    {
        public CreateOrUpdateTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Type name is required")
                .MaximumLength(100).WithMessage("Type name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-&.]+$").WithMessage("Type name contains invalid characters");
        }
    }
}