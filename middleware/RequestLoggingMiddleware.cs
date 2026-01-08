using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ecommerce.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;

            _logger.LogInformation("Starting request: {Method} {Path} at {Time}", 
                requestMethod, requestPath, DateTime.UtcNow);

            try
            {
                await _next(context);
                
                stopwatch.Stop();
                _logger.LogInformation("Completed request: {Method} {Path} - Status: {StatusCode} - Duration: {Elapsed}ms",
                    requestMethod, requestPath, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Request failed: {Method} {Path} - Duration: {Elapsed}ms",
                    requestMethod, requestPath, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}