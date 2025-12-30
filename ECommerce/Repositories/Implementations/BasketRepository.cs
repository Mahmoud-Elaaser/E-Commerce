using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using StackExchange.Redis;
using System.Text.Json;


namespace ECommerce.Repositories.Implementations
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        private const string BasketKeyPrefix = "basket:";

        public BasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Basket?> GetBasketAsync(string basketId)
        {
            var redisKey = $"{BasketKeyPrefix}{basketId}";
            var data = await _database.StringGetAsync(redisKey);

            if (data.IsNullOrEmpty)
                return null;

            var basket = JsonSerializer.Deserialize<Basket>(data!);
            if (basket != null)
            {
                basket.Id = basketId;
            }

            return basket;
        }

        public async Task<Basket?> UpdateBasketAsync(Basket basket)
        {
            // Generate ID if not provided
            if (string.IsNullOrEmpty(basket.Id))
            {
                basket.Id = Guid.NewGuid().ToString();
            }

            var redisKey = $"{BasketKeyPrefix}{basket.Id}";
            var json = JsonSerializer.Serialize(basket);

            var created = await _database.StringSetAsync(
                redisKey,
                json,
                TimeSpan.FromDays(30)
            );

            return created ? basket : null;
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            var redisKey = $"{BasketKeyPrefix}{basketId}";
            return await _database.KeyDeleteAsync(redisKey);
        }

        public async Task<bool> ClearBasketAsync(string basketId)
        {
            var redisKey = $"{BasketKeyPrefix}{basketId}";
            var basket = await GetBasketAsync(basketId);

            if (basket == null)
                return false;

            basket.Items.Clear();

            var json = JsonSerializer.Serialize(basket);
            var result = await _database.StringSetAsync(redisKey, json, TimeSpan.FromDays(30));

            return result;
        }

        public async Task<Basket?> CreateOrUpdateBasketAsync(Basket basket, TimeSpan? timeToLive = null)
        {
            if (string.IsNullOrEmpty(basket.Id))
            {
                basket.Id = Guid.NewGuid().ToString();
            }

            var redisKey = $"{BasketKeyPrefix}{basket.Id}";
            var jsonBasket = JsonSerializer.Serialize(basket);

            var isCreatedorUpdate = await _database.StringSetAsync(
                redisKey,
                jsonBasket,
                timeToLive ?? TimeSpan.FromDays(30)
            );

            return isCreatedorUpdate ? basket : null;
        }

        public async Task<bool> ClearAllBasketsAsync()
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{BasketKeyPrefix}*").ToArray();

                if (keys.Length == 0)
                    return true;

                var redisKeys = keys.Select(key => (RedisKey)key).ToArray();
                var deletedCount = await _database.KeyDeleteAsync(redisKeys);

                return deletedCount > 0;
            }
            catch (Exception ex)
            {
                // Log the exception (you should inject ILogger in production)
                Console.WriteLine($"Error clearing all baskets: {ex.Message}");
                return false;
            }
        }
    }
}
