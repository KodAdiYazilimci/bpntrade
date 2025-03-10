using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Roots;

using Microsoft.AspNetCore.Diagnostics;

using Newtonsoft.Json;

using System.Net;

namespace BpnTrade.Api.Handlers
{
    public static class GlobalExceptionHandler
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    Exception error = context.Features.Get<IExceptionHandlerPathFeature>().Error;

                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";

                    await context
                    .Response.
                    WriteAsync(
                            JsonConvert
                            .SerializeObject(
                                ResultRoot.Failure(
                                    new ErrorDto(
                                        "UNX",
                                        "Beklenmeyen bir hata oluştu",
                                        new ErrorDto("EXC", $"{error.Message} {error.InnerException?.Message}")))));
                });
            });

            return app;
        }
    }
}
