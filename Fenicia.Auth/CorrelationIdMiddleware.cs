namespace Fenicia.Auth;

using Serilog;
using Serilog.Context;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        const string CorrelationIdHeader = "X-Correlation-ID";

        try
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Add(CorrelationIdHeader, correlationId);
                Log.Information(messageTemplate: "Generated new correlation ID: {CorrelationId}", correlationId);
            }

            context.Response.Headers[CorrelationIdHeader] = correlationId;
            using (LogContext.PushProperty(name: "CorrelationId", correlationId))
            {
                await next(context).ConfigureAwait(continueOnCapturedContext: false);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, messageTemplate: "Error processing request with correlation ID: {CorrelationId}", context.Request.Headers[CorrelationIdHeader]);
            throw;
        }
    }
}
