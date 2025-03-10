using AutoMapper;

namespace BpnTrade.Api.DI
{
    public static class MappingInjection
    {
        public static IServiceCollection RegisterMapper(this IServiceCollection services)
        {
            services.AddSingleton(new MapperConfiguration(config =>
            {
                config.AddProfile(new MappingProfile());
            }).CreateMapper());

            return services;
        }
    }
}
