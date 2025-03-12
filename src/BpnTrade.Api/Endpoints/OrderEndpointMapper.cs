using BpnTrade.Api.Validations.Order;
using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Services;

using Microsoft.AspNetCore.Mvc;

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
            builder.MapPut("/", async ([FromBody] CreateOrderRequestDto dto, [FromServices] IOrderService orderService, [FromServices] IHttpContextAccessor httpContextAccessor) =>
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var validationResult = await CreateOrderValidator.Instance.ValidateAsync(dto, cancellationTokenSource.Token);

                if (!validationResult.IsSuccess)
                    return Results.BadRequest(validationResult.Error);

                dto.UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId").Value;

                var result = await orderService.CreateAsync(dto, cancellationTokenSource.Token);

                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Error);
            }).RequireAuthorization();

            return builder;
        }

        public static RouteGroupBuilder MapCompleteOrderEndpoint(this RouteGroupBuilder builder)
        {
            builder.MapPost("/", async ([FromBody] CompleteOrderRequestDto dto, [FromServices] IOrderService orderService, [FromServices] IHttpContextAccessor httpContextAccessor) =>
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                var validationResult = await CompleteOrderValidator.Instance.ValidateAsync(dto, cancellationTokenSource.Token);

                if (!validationResult.IsSuccess)
                    return Results.BadRequest(validationResult.Error);

                dto.UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId").Value;

                var result = await orderService.CompleteOrderAsync(dto, cancellationTokenSource.Token);

                return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Error);
            }).RequireAuthorization();

            return builder;
        }
    }
}
