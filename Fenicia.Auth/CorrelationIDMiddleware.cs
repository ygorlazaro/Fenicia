using Serilog;
using Serilog.Context;

namespace Fenicia.Auth;

public  class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        const string correlationIdHeader = "X-Correlation-ID";

        try
        {
            if (!context.Request.Headers.TryGetValue(correlationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Append(correlationIdHeader, correlationId);
                Log.Information("Generated new correlation ID: {CorrelationId}", correlationId);
            }

            context.Response.Headers[correlationIdHeader] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next(context).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing request with correlation ID: {CorrelationId}",
                context.Request.Headers[correlationIdHeader]);
            throw;
        }
    }
}