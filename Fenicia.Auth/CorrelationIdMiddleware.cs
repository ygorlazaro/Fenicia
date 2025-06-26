using Serilog;
using Serilog.Context;

namespace Fenicia.Auth;

/// <summary>
/// Middleware that handles correlation ID for request tracing.
/// </summary>
/// <param name="next">The next middleware delegate in the pipeline.</param>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Processes HTTP request by adding correlation ID to the request and response headers.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        const string CorrelationIdHeader = "X-Correlation-ID";

        try
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Add(CorrelationIdHeader, correlationId);
                Log.Information("Generated new correlation ID: {CorrelationId}", correlationId);
            }

            context.Response.Headers[CorrelationIdHeader] = correlationId;
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next(context).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing request with correlation ID: {CorrelationId}",
                context.Request.Headers[CorrelationIdHeader]);
            throw;
        }
    }
}
