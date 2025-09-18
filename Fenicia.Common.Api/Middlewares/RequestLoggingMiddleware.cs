namespace Fenicia.Common.API.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestLoggingMiddleware
{
    private readonly ILogger logger;
    private readonly RequestDelegate next;

    public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        this.next = next;
        this.logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await this.next(context).ConfigureAwait(continueOnCapturedContext: false);
        }
        finally
        {
            this.logger.LogInformation("{Date} Request {Method} {Url} {Params} {Ip} => {StatusCode}", DateTimeOffset.Now, context.Request.Method, context.Request.Path.Value, context.Request.QueryString, context.Connection.RemoteIpAddress?.ToString(), context.Response != null ? context.Response.StatusCode : null);
        }
    }
}
