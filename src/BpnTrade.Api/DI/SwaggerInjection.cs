using Microsoft.OpenApi.Models;

namespace BpnTrade.Api.DI
{
    public static class SwaggerInjection
    {
        public static IServiceCollection RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(setup =>
            {
                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }
    }
}
