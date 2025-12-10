using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using StackExchange.Redis;
using System.Text.Json;


namespace ECommerce.Repositories.Implementations
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Basket?> GetBasketAsync(string basketId)
        {
            var data = await _database.StringGetAsync(basketId);

            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Basket>(data!);
        }

        public async Task<Basket?> UpdateBasketAsync(Basket basket)
        {
            var json = JsonSerializer.Serialize(basket);

            var created = await _database.StringSetAsync(
                basket.Id,
                json,
                TimeSpan.FromDays(30)
            );

            return created ? await GetBasketAsync(basket.Id) : null;
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<bool> ClearBasketAsync(string basketId)
        {
            if (string.IsNullOrWhiteSpace(basketId))
                return false;

            var basket = await GetBasketAsync(basketId);

            if (basket == null)
                return false;

            // Clear items but keep other properties
            basket.Items.Clear();

            var result = await UpdateBasketAsync(basket);

            return result != null;

        }




        public async Task<Basket?> CreateOrUpdateBasketAsync(Basket basket, TimeSpan? timeToLive = null)
        {
            var jsonBasket = JsonSerializer.Serialize(basket);
            var isCreatedorUpdate = await _database.StringSetAsync(basket.Id, jsonBasket, timeToLive ?? TimeSpan.FromDays(30));

            return isCreatedorUpdate ? await GetBasketAsync(basket.Id) : null;
        }
    }
}
