namespace Fenicia.Auth;

using Serilog;
using Serilog.Context;

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        const string correlationIdHeader = "X-Correlation-ID";

        try
        {
            if (!context.Request.Headers.TryGetValue(correlationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Add(correlationIdHeader, correlationId);
                Log.Information("Generated new correlation ID: {CorrelationId}", correlationId);
            }

            context.Response.Headers[correlationIdHeader] = correlationId;
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context).ConfigureAwait(continueOnCapturedContext: false);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing request with correlation ID: {CorrelationId}", context.Request.Headers[correlationIdHeader]);
            throw;
        }
    }
}
