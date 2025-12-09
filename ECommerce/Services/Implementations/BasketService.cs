using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;

namespace ECommerce.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;

        public BasketService(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<Basket?> GetBasketAsync(string basketId)
        {
            return await _basketRepository.GetBasketAsync(basketId);
        }

        public async Task<Basket?> CreateOrGetBasketAsync(string? basketId = null)
        {
            if (string.IsNullOrEmpty(basketId))
            {
                var newBasket = new Basket();
                return await _basketRepository.UpdateBasketAsync(newBasket);
            }

            var basket = await _basketRepository.GetBasketAsync(basketId);
            return basket ?? await _basketRepository.UpdateBasketAsync(new Basket(basketId));
        }

        public async Task<Basket?> AddItemToBasketAsync(string basketId, BasketItem item)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                basket.Items.Add(item);
            }

            return await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task<Basket?> UpdateItemQuantityAsync(string basketId, int productId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return basket;

            if (quantity == 0)
            {
                basket.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            return await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task<Basket?> RemoveItemFromBasketAsync(string basketId, int productId)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                basket.Items.Remove(item);
            }

            return await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task<Basket?> UpdateDeliveryMethodAsync(string basketId, int deliveryMethodId, decimal shippingPrice)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            basket.DeliveryMethodId = deliveryMethodId;
            basket.ShippingPrice = shippingPrice;

            return await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task<Basket?> SetPaymentIntentAsync(string basketId, string paymentIntentId, string clientSecret)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            basket.PaymentIntentId = paymentIntentId;
            basket.ClientSecret = clientSecret;

            return await _basketRepository.UpdateBasketAsync(basket);
        }

        public async Task<bool> ClearBasketAsync(string basketId)
        {
            return await _basketRepository.DeleteBasketAsync(basketId);
        }

        public async Task<decimal> GetBasketTotalAsync(string basketId)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return 0;

            var itemsTotal = basket.Items.Sum(item => item.Price * item.Quantity);
            return itemsTotal + basket.ShippingPrice;
        }
    }
}