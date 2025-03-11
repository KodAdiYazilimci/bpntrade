using BpnTrade.Domain.Services;

using Microsoft.AspNetCore.Mvc;

namespace BpnTrade.Api.Endpoints
{
    public static class ProductEndpointMapper
    {
        public static RouteGroupBuilder MapProductEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/product")
                .MapGetProductsEndpoint();

            return group;
        }

        private static RouteGroupBuilder MapGetProductsEndpoint(this RouteGroupBuilder builder)
        {
            builder.MapGet("/", async ([FromServices] IProductService productService) =>
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var result = await productService.GetProductsAsync(cancellationTokenSource.Token);

                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Error);
            }).RequireAuthorization();

            return builder;
        }
    }
}
