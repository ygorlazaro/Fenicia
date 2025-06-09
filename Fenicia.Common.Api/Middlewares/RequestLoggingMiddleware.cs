using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fenicia.Common.Api.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();

    public async Task Invoke(HttpContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        try
        {
            await next(context).ConfigureAwait(false);
        }
        finally
        {
            _logger.LogInformation(
                "{Date} Request {Method} {Url} {Params} {Ip} => {StatusCode}",
                DateTimeOffset.Now,
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Request?.QueryString,
                context.Connection?.RemoteIpAddress?.ToString(),
                context.Response?.StatusCode
            );
        }
    }
}
