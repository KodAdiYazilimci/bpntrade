using BpnTrade.App.Services;
using BpnTrade.Domain.Services;

namespace BpnTrade.Api.DI
{
    public static class ServiceInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
