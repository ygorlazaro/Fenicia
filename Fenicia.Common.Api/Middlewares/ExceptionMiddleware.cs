using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Fenicia.Common.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(
            JsonConvert.SerializeObject(
                new { StatusCode = context.Response.StatusCode, Message = ex.Message }
            )
        );
    }
}
