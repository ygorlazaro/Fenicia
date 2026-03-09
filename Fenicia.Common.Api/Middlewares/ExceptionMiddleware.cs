using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var problem = new ProblemDetails
            {
                Title = "Erro interno",
                Status = context.Response.StatusCode,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
