using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace BpnTrade.Api.DI
{
    public static class SecurityInjection
    {
        public static IServiceCollection RegisterJwt(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var serviceProvider = builder.Services.BuildServiceProvider();

                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                    var secret = configuration.GetSection("Security")["JwtSecret"];
                    var issuer = configuration.GetSection("Security")["Issuer"];
                    var audience = configuration.GetSection("Security")["Audience"];

                    options.Authority = issuer;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = audience,
                        ValidIssuer = issuer,
                        RequireExpirationTime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret))
                    };
                });

            return services;
        }
    }
}
