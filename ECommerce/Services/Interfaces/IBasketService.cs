using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IBasketService
    {
        Task<Basket?> GetBasketAsync(string basketId);
        Task<Basket?> CreateOrGetBasketAsync(string? basketId = null);
        Task<Basket?> AddItemToBasketAsync(string basketId, BasketItem item);
        Task<Basket?> UpdateItemQuantityAsync(string basketId, int productId, int quantity);
        Task<Basket?> RemoveItemFromBasketAsync(string basketId, int productId);
        Task<Basket?> UpdateDeliveryMethodAsync(string basketId, int deliveryMethodId, decimal shippingPrice);
        Task<Basket?> SetPaymentIntentAsync(string basketId, string paymentIntentId, string clientSecret);
        Task<bool> ClearBasketAsync(string basketId);
        Task<decimal> GetBasketTotalAsync(string basketId);
    }
}
