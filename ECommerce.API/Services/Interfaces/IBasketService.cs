namespace ECommerce.API.Services.Interfaces
{
    public interface IBasketService
    {
        Task<CustomerBasket?> GetBasketAsync(string basketId);
        Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string basketId);
        Task<CustomerBasket?> AddItemAsync(string basketId, AddToBasketDto item);
        Task<CustomerBasket?> UpdateItemAsync(string basketId, UpdateBasketItemDto item);
        Task<CustomerBasket?> RemoveItemAsync(string basketId, Guid productId, Guid? variantId);
        Task<CustomerBasket?> ApplyDiscountAsync(string basketId, string discountCode);
    }
}
