
using BpnTrade.Api.DI;
using BpnTrade.Api.Endpoints;
using BpnTrade.Api.Handlers;
using BpnTrade.Api.Middlewares;

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

            builder.Services
                .RegisterSwagger()
                .AddHttpClient()
                .AddMemoryCache()
                .AddHttpContextAccessor()
                .RegisterMapper()
                .RegisterDbContext()
                .RegisterRepositories()
                .RegisterUnitOfWork()
                .RegisterServices()
                .RegisterProductProviders()
                .RegisterJwt(builder);

            var app = builder.Build();

            app.UseGlobalExceptionHandler()
                .UseLoggerMiddleware();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapOrderEndpoints();
            app.MapProductEndpoints();
            app.MapUserEndpoints(app.Services.GetRequiredService<IConfiguration>());

            app.MapControllers();

            app.Run();
        }
    }
}
