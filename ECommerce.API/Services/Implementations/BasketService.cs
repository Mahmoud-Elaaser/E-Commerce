using ECommerce.API.DTOs;
using ECommerce.API.Models.Basket;
using ECommerce.API.Repositories.Interfaces;
using ECommerce.API.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.API.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly StackExchange.Redis.IDatabase _database;

        public BasketService(
            IConnectionMultiplexer redis,
            IProductRepository productRepository,
            IDiscountRepository discountRepository)
        {
            _redis = redis;
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _database = redis.GetDatabase();
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var data = await _database.StringGetAsync(basketId);

            if (data.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<CustomerBasket>(data!);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            basket.LastModified = DateTime.UtcNow;

            var json = JsonSerializer.Serialize(basket);
            var created = await _database.StringSetAsync(
                basket.Id,
                json,
                TimeSpan.FromDays(30) // Basket expires after 30 days
            );

            if (!created)
                return null;

            return await GetBasketAsync(basket.Id);
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket?> AddItemAsync(string basketId, AddToBasketDto itemDto)
        {
            var basket = await GetBasketAsync(basketId) ?? new CustomerBasket { Id = basketId };

            // Get product details
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product is null)
                throw new ArgumentException("Product not found");

            // Check stock availability
            var availableStock = product.QuantityInStock;
            if (itemDto.ProductVariantId.HasValue)
            {
                var variant = product.Variants.FirstOrDefault(v => v.Id == itemDto.ProductVariantId);
                if (variant == null)
                    throw new ArgumentException("Product variant not found");
                availableStock = variant.Quantity;
            }

            if (availableStock < itemDto.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            // Check if item already exists in basket
            var existingItem = basket.Items.FirstOrDefault(i =>
                i.ProductId == itemDto.ProductId &&
                i.ProductVariantId == itemDto.ProductVariantId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + itemDto.Quantity;
                if (newQuantity > availableStock)
                    throw new InvalidOperationException("Requested quantity exceeds available stock");

                existingItem.Quantity = newQuantity;
            }
            else
            {
                var price = product.DiscountedPrice ?? product.Price;
                var variantDescription = string.Empty;

                if (itemDto.ProductVariantId.HasValue)
                {
                    var variant = product.Variants.First(v => v.Id == itemDto.ProductVariantId);
                    price += variant.AdditionalPrice;
                    variantDescription = $"{variant.Color}, {variant.Size}";
                }

                basket.Items.Add(new BasketItem
                {
                    ProductId = itemDto.ProductId,
                    ProductName = product.Name,
                    ProductVariantId = itemDto.ProductVariantId,
                    VariantDescription = variantDescription,
                    Quantity = itemDto.Quantity,
                    Price = price,
                    ImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                    MaxQuantity = availableStock
                });
            }

            return await UpdateBasketAsync(basket);
        }

        public async Task<CustomerBasket?> UpdateItemAsync(string basketId, UpdateBasketItemDto itemDto)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            var item = basket.Items.FirstOrDefault(i =>
                i.ProductId == itemDto.ProductId &&
                i.ProductVariantId == itemDto.ProductVariantId);

            if (item == null)
                throw new ArgumentException("Item not found in basket");

            if (itemDto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            if (itemDto.Quantity > item.MaxQuantity)
                throw new InvalidOperationException("Requested quantity exceeds available stock");

            item.Quantity = itemDto.Quantity;

            return await UpdateBasketAsync(basket);
        }

        public async Task<CustomerBasket?> RemoveItemAsync(string basketId, Guid productId, Guid? variantId)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            var item = basket.Items.FirstOrDefault(i =>
                i.ProductId == productId &&
                i.ProductVariantId == variantId);

            if (item != null)
            {
                basket.Items.Remove(item);
                return await UpdateBasketAsync(basket);
            }

            return basket;
        }

        public async Task<CustomerBasket?> ApplyDiscountAsync(string basketId, string discountCode)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null;

            var discount = await _discountRepository.GetByCodeAsync(discountCode);

            if (discount is null)
                throw new ArgumentException("Invalid discount code");

            if (discount.StartDate > DateTime.UtcNow || discount.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException("Discount code is not valid at this time");

            if (discount.UseLimit.HasValue && discount.UsedCount >= discount.UseLimit.Value)
                throw new InvalidOperationException("Discount code has reached its usage limit");

            var subtotal = basket.GetSubtotal();

            if (discount.MinimumOrderAmount.HasValue && subtotal < discount.MinimumOrderAmount.Value)
                throw new InvalidOperationException($"Minimum order amount of {discount.MinimumOrderAmount:C} required");

            // Calculate discount amount
            var discountAmount = discount.DiscountType == DiscountType.Percentage
                ? subtotal * (discount.Value / 100)
                : discount.Value;

            if (discount.MaximumDiscountAmount.HasValue && discountAmount > discount.MaximumDiscountAmount.Value)
                discountAmount = discount.MaximumDiscountAmount.Value;

            basket.DiscountCode = discountCode;
            basket.DiscountAmount = discountAmount;

            return await UpdateBasketAsync(basket);
        }
    }
}
