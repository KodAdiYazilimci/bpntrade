using BpnTrade.App.Adapters;
using BpnTrade.Domain.Adapters;

namespace BpnTrade.Api.DI
{
    public static class IntegrationInjection
    {
        public static IServiceCollection RegisterProductProviders(this IServiceCollection services)
        {
            services.AddScoped<IProductAdapter, BpnProductAdapter>();

            return services;
        }
    }
}
