using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fenicia.Common.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
{
    private readonly ILogger logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();

    public async Task Invoke(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await next(context).ConfigureAwait(continueOnCapturedContext: false);
        }
        finally
        {
            logger.LogInformation("{Date} Request {Method} {Url} {Params} {Ip} => {StatusCode}", DateTimeOffset.Now, context.Request.Method, context.Request.Path.Value, context.Request.QueryString, context.Connection.RemoteIpAddress?.ToString(), context.Response.StatusCode);
        }
    }
}
