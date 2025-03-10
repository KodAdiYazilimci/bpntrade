using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Services;

namespace BpnTrade.Api.Endpoints
{
    public static class OrderEndpointMapper
    {
        public static RouteGroupBuilder MapOrderEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/order")
                .MapCreateOrderEndpoint()
                .MapCompleteOrderEndpoint();

            return group;
        }

        public static RouteGroupBuilder MapCreateOrderEndpoint(this RouteGroupBuilder builder)
        {
            builder.MapPut("/", async (CreateOrderRequestDto dto, IOrderService orderService, HttpContextAccessor httpContextAccessor) =>
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                dto.UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.ValueType == "UserId").Value;

                var result = await orderService.CreateAsync(dto, cancellationTokenSource.Token);

                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Error);
            }).RequireAuthorization();

            return builder;
        }

        public static RouteGroupBuilder MapCompleteOrderEndpoint(this RouteGroupBuilder builder)
        {
            builder.MapPost("/", async (CompleteOrderRequestDto dto, IOrderService orderService, HttpContextAccessor httpContextAccessor) =>
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                dto.UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.ValueType == "UserId").Value;

                var result = await orderService.CompleteOrderAsync(dto, cancellationTokenSource.Token);

                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Error);
            }).RequireAuthorization();

            return builder;
        }
    }
}
