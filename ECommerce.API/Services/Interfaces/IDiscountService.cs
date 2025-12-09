namespace ECommerce.API.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<Discount?> ValidateDiscountCodeAsync(string code, decimal orderAmount);
    }

}
