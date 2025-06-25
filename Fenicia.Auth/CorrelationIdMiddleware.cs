using Serilog.Context;

namespace Fenicia.Auth;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        const string CorrelationIdHeader = "X-Correlation-ID";

        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers.Add(CorrelationIdHeader, correlationId);
        }

        context.Response.Headers[CorrelationIdHeader] = correlationId;
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
