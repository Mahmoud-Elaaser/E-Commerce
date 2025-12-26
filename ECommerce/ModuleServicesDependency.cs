using ECommerce.Data;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Repositories.Implementations;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Implementations;
using ECommerce.Services.Interfaces;
using ECommerce.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using FileService = ECommerce.Helpers.FileService;
using ProductService = ECommerce.Services.Implementations.ProductService;

namespace ECommerce
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
            /// AutoMapper Configuration
            services.AddAutoMapper(config =>
            {
                config.AllowNullCollections = true;
                config.AllowNullDestinationValues = true;
            }, typeof(Program).Assembly);


            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.Configure<AdminOptions>(configuration.GetSection("AdminOptions"));

            services.AddJwtOptions(configuration);
        }

        public static void AddJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions!.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                };
            });
            services.AddAuthorization();

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
