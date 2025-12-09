
using StackExchange.Redis;

namespace ECommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            /// Setup Redis Connection
            var redisConfiguration = builder.Configuration.GetConnectionString("Redis");
            var redis = ConnectionMultiplexer.Connect(redisConfiguration!);
            builder.Services.AddSingleton<IConnectionMultiplexer>(redis);


            builder.Services.AddServiceDependencies();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
