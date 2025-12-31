using ECommerce.DTOs.Product;
using FluentValidation;

namespace ECommerce.Validators.Product
{
    public class UpdateStockDtoValidator : AbstractValidator<UpdateStockDto>
    {
        public UpdateStockDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
                .LessThanOrEqualTo(10000).WithMessage("Quantity cannot exceed 10,000 units");
        }
    }
}