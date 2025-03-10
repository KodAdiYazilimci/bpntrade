using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BpnTrade.Api.Endpoints
{
    public static class UserEndpointMapper
    {
        public static RouteGroupBuilder MapProductEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/user")
                .MapGetProductsEndpoint();

            return group;
        }

        public static RouteGroupBuilder MapLoginEndpoint(this RouteGroupBuilder builder, IConfiguration configuration)
        {
            builder.MapPost("/login", async () =>
            {
                var secret = configuration.GetSection("Security")["JwtSecret"];
                var issuer = configuration.GetSection("Security")["Issuer"];
                var audience = configuration.GetSection("Security")["Audience"];

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

                var jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: new List<Claim> {
                             new Claim("UserId", "1")
                    },
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256));

                return Task.FromResult(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                    TokenExpireDate = DateTime.Now.AddHours(1)
                });
            }).AllowAnonymous();

            return builder;
        }
    }
}
