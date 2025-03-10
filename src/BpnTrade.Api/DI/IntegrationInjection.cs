using BpnTrade.App.Adapters;
using BpnTrade.App.Facades;
using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Facades;

namespace BpnTrade.Api.DI
{
    public static class IntegrationInjection
    {
        public static IServiceCollection RegisterProductProviders(this IServiceCollection services)
        {
            services
                .AddScoped<IProductAdapter, BpnProductAdapter>()
                .AddScoped<IBalanceAdapter, BalanceAdapter>()
                .AddScoped<ICompleteAdapter, CompleteAdapter>()
                .AddScoped<IPreOrderAdapter, PreOrderAdapter>()
                .AddScoped<ICancelAdapter, CancelAdapter>();

            services.AddScoped<IPaymentFacade, BpnPaymentFacade>();

            return services;
        }
    }
}
