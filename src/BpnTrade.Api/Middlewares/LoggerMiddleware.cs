using System.Diagnostics;

namespace BpnTrade.Api.Middlewares
{
    public interface IHttpRequestTimeFeature
    {
        DateTime RequestTime { get; }
    }

    public class HttpRequestTimeFeature : IHttpRequestTimeFeature
    {
        public DateTime RequestTime { get; }

        public HttpRequestTimeFeature()
        {
            RequestTime = DateTime.UtcNow;
        }
    }

    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var httpRequestTimeFeature = new HttpRequestTimeFeature();

            httpContext.Features.Set<IHttpRequestTimeFeature>(httpRequestTimeFeature);

            var watch = new Stopwatch();
            watch.Start();

            string request = string.Empty;
            string response = string.Empty;

            try
            {
                httpContext.Request.EnableBuffering();

                using (StreamReader streamReader = new StreamReader(httpContext.Request.Body, leaveOpen: true))
                {
                    request = await streamReader.ReadToEndAsync();

                    httpContext.Request.Body.Position = 0;
                }

                var originalBody = httpContext.Response.Body;
                using (var newBody = new MemoryStream())
                {
                    httpContext.Response.Body = newBody;

                    try
                    {
                        await _next(httpContext);
                    }
                    finally
                    {
                        newBody.Seek(0, SeekOrigin.Begin);
                        response = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
                        newBody.Seek(0, SeekOrigin.Begin);
                        await newBody.CopyToAsync(originalBody);
                    }
                }
            }
            catch { }

            httpContext.Response.OnCompleted(async () =>
            {
                watch.Stop();

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                try
                {
                    // TODO: Buraya logger implemente edilecek
                }
                catch (Exception)
                {

                }
            });
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggerMiddleware>();
        }
    }
}
