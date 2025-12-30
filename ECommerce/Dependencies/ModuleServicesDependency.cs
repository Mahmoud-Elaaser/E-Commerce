using ECommerce.Data;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Repositories.Implementations;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Implementations;
using ECommerce.Services.Interfaces;
using ECommerce.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using ProductService = ECommerce.Services.Implementations.ProductService;

namespace ECommerce.Dependencies
{
    public static class ModuleServicesDependency
    {
        public static void AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                //options.UseSqlServer(configuration.GetConnectionString("MonsterConnection"));
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            /// Setup Redis Connection
            var redisConfiguration = configuration.GetConnectionString("Redis");
            var redis = ConnectionMultiplexer.Connect(redisConfiguration!);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            /// Repositories & services registration
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IProductBrandService, ProductBrandService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IEmailService, EmailService>();

            /// AutoMapper Configuration
            services.AddAutoMapper(config =>
            {
                config.AllowNullCollections = true;
                config.AllowNullDestinationValues = true;
            }, typeof(Program).Assembly);


            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.Configure<AdminOptions>(configuration.GetSection("AdminOptions"));
            services.Configure<StripeOptions>(configuration.GetSection("StripeOptions"));
            services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));

        }



        public static async Task<WebApplication> SeedDbAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await IdentityDataInitializer.SeedAsync(services);
            }

            return app;
        }
    }
}
