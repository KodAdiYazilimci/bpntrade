using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Services;

namespace BpnTrade.Api.Endpoints
{
    public static class OrderEndpointMapper
    {
        public static RouteGroupBuilder MapOrderEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/order");

            return group;
        }

        public static RouteGroupBuilder MapCreateOrderEndpoint(this RouteGroupBuilder builder)
        {
            builder.MapPut("/", async (CreateOrderRequestDto dto, IOrderService orderService) =>
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var result = await orderService.CreateAsync(dto, cancellationTokenSource.Token);

                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Error);
            });

            return builder;
        }
    }
}
