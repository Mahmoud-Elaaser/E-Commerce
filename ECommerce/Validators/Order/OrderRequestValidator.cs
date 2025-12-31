using ECommerce.DTOs.Order;
using FluentValidation;

namespace ECommerce.Validators.Order
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.BasketId)
                .NotEmpty().WithMessage("Basket ID is required")
                .MaximumLength(100).WithMessage("Basket ID cannot exceed 100 characters");

            RuleFor(x => x.ShipToAddress)
                .NotNull().WithMessage("Shipping address is required")
                .SetValidator(new AddressDtoValidator());

            RuleFor(x => x.DeliveryMethodId)
                .GreaterThan(0).WithMessage("Delivery method ID must be greater than zero");
        }
    }
}