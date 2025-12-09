using ECommerce.Data;
using ECommerce.Repositories.Implementations;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Implementations;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ECommerce
{
    public static class ModuleServicesDependency
    {
        public static void AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });


            /// Setup Redis Connection
            var redisConfiguration = configuration.GetConnectionString("Redis");
            var redis = ConnectionMultiplexer.Connect(redisConfiguration!);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            /// Repositories & services registration
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketService, BasketService>();



        }
    }
}
