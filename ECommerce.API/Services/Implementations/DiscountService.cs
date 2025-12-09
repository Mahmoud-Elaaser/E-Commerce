using ECommerce.API.Services.Interfaces;

namespace ECommerce.API.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<Discount?> ValidateDiscountCodeAsync(string code, decimal orderAmount)
        {
            var discount = await _discountRepository.GetByCodeAsync(code);

            if (discount == null || !discount.IsActive)
                return null;

            if (discount.StartDate > DateTime.UtcNow || discount.EndDate < DateTime.UtcNow)
                return null;

            if (discount.UseLimit.HasValue && discount.UsedCount >= discount.UseLimit.Value)
                return null;

            if (discount.MinimumOrderAmount.HasValue && orderAmount < discount.MinimumOrderAmount.Value)
                return null;

            return discount;
        }
    }

}
