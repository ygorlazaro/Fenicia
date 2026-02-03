using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fenicia.Common.Api.Middlewares;

public sealed class WideEventMiddleware(
    RequestDelegate next,
    ILogger<WideEventMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        WideEventContext wide)
    {
        var sw = Stopwatch.StartNew();

        wide.Path = context.Request.Path;
        wide.Method = context.Request.Method;

        try
        {
            await next(context);

            wide.StatusCode = context.Response.StatusCode;
            wide.Success = context.Response.StatusCode < 500;
        }
        catch (Exception ex)
        {
            wide.Success = false;
            wide.StatusCode = 500;
            wide.ErrorMessage = ex.Message;
            wide.ErrorCode = ex.GetType().Name;

            throw;
        }
        finally
        {
            sw.Stop();
            wide.DurationMs = sw.ElapsedMilliseconds;

            logger.LogInformation(
                "wide_event{Args}",
                new
                {
                    wide.TraceId,
                    wide.Path,
                    wide.Method,
                    wide.StatusCode,
                    wide.DurationMs,
                    wide.Success,
                    wide.UserId,
                    wide.Operation,
                    wide.ErrorCode,
                    wide.ErrorMessage
                });
        }
    }
}
