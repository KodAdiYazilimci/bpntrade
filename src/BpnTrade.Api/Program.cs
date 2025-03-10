
using BpnTrade.Api.DI;
using BpnTrade.Api.Endpoints;
using BpnTrade.Api.Handlers;
using BpnTrade.Api.Middlewares;
using BpnTrade.App.Persistence;

using Microsoft.EntityFrameworkCore;

namespace BpnTrade.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            builder.Services.RegisterMapper();
            builder.Services.RegisterDbContext();
            builder.Services.RegisterRepositories();
            builder.Services.RegisterUnitOfWork();
            builder.Services.RegisterServices();
            builder.Services.RegisterProductProviders();

            builder.Services.AddDbContext<BpnContext>(options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();

                var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Default");

                options.UseSqlServer(connectionString);
            });

            var app = builder.Build();

            app.UseGlobalExceptionHandler();
            app.UseLoggerMiddleware();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapOrderEndpoints();
            app.MapProductEndpoints();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
