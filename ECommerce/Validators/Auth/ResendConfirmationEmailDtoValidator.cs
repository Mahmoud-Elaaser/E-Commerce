using ECommerce.DTOs.Auth;
using FluentValidation;

namespace ECommerce.Validators.Auth
{
    public class ResendConfirmationEmailDtoValidator : AbstractValidator<ResendConfirmationEmailDto>
    {
        public ResendConfirmationEmailDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}