using ECommerce.DTOs.Basket;
using FluentValidation;

namespace ECommerce.Validators.Basket
{
    public class BasketItemDtoValidator : AbstractValidator<BasketItemDTO>
    {
        public BasketItemDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero");

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.PictureUrl)
                .NotEmpty().WithMessage("Picture URL is required")
                .MaximumLength(500).WithMessage("Picture URL cannot exceed 500 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 99).WithMessage("Quantity must be between 1 and 99");
        }
    }
}