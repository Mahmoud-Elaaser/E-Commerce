using ECommerce.DTOs.Basket;
using FluentValidation;

namespace ECommerce.Validators.Basket
{
    public class CustomerBasketDtoValidator : AbstractValidator<CustomerBasketDto>
    {
        public CustomerBasketDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Basket ID is required")
                .MaximumLength(100).WithMessage("Basket ID cannot exceed 100 characters");

            RuleFor(x => x.DeliveryMethodId)
                .GreaterThan(0).WithMessage("Delivery method ID must be greater than zero")
                .When(x => x.DeliveryMethodId.HasValue);

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items collection cannot be null")
                .Must(items => items != null && items.Any())
                .WithMessage("Basket must contain at least one item")
                .When(x => x.Items != null);

            RuleForEach(x => x.Items)
                .SetValidator(new BasketItemDtoValidator())
                .When(x => x.Items != null && x.Items.Any());

        }
    }
}