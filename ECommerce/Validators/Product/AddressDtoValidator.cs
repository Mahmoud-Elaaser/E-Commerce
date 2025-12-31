using ECommerce.DTOs;
using FluentValidation;

namespace ECommerce.Validators
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("First name contains invalid characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Last name contains invalid characters");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required")
                .MaximumLength(200).WithMessage("Street cannot exceed 200 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("City contains invalid characters");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Country contains invalid characters");
        }
    }
}