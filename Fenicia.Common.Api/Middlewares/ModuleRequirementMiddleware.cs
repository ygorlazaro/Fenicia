using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.API.Middlewares;

public class ModuleRequirementMiddleware(RequestDelegate next, string requiredModule)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var moduleClaim = context.User.FindFirst("module");

        if (moduleClaim == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Missing 'module' claim.");

            return;
        }

        try
        {
            var modules = JsonSerializer.Deserialize<List<string>>(moduleClaim.Value);

            if (modules is null || !modules.Contains(requiredModule, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Access to module '{requiredModule}' is forbidden.");

                return;
            }
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Invalid 'module' claim format.");

            return;
        }

        await next(context);
    }
}