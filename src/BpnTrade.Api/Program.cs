
using BpnTrade.Api.DI;
using BpnTrade.Api.Endpoints;
using BpnTrade.Api.Handlers;
using BpnTrade.Api.Middlewares;
using BpnTrade.App.Persistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;

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
            builder.Services.AddSwaggerGen(setup =>
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

            builder.Services.AddHttpClient();
            builder.Services.AddMemoryCache();

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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            var app = builder.Build();

            app.UseGlobalExceptionHandler();
            app.UseLoggerMiddleware();

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
