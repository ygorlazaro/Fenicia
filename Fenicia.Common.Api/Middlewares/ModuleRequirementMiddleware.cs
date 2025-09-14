namespace Fenicia.Common.Api.Middlewares;

using System.Text.Json;

using Microsoft.AspNetCore.Http;

public class ModuleRequirementMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _requiredModule;

    public ModuleRequirementMiddleware(RequestDelegate next, string requiredModule)
    {
        _next = next;
        _requiredModule = requiredModule;
    }

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

            if (modules is null || !modules.Contains(_requiredModule, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Access to module '{_requiredModule}' is forbidden.");
                return;
            }
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Invalid 'module' claim format.");
            return;
        }

        await _next(context);
    }
}
