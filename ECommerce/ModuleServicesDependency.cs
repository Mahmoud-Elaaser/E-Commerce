using ECommerce.Repositories.Implementations;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Implementations;
using ECommerce.Services.Interfaces;

namespace ECommerce
{
    public static class ModuleServicesDependency
    {
        public static void AddServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketService, BasketService>();
        }
    }
}
