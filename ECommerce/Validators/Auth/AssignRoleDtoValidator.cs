using ECommerce.DTOs.Auth;
using FluentValidation;

namespace ECommerce.Validators.Auth
{
    public class AssignRoleDtoValidator : AbstractValidator<AssignRoleDto>
    {
        public AssignRoleDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters");
        }
    }
}