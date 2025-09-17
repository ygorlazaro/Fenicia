namespace Fenicia.Common.API.Middlewares;

using System.Text.Json;

using Microsoft.AspNetCore.Http;

public class ModuleRequirementMiddleware
{
    private readonly RequestDelegate next;
    private readonly string requiredModule;

    public ModuleRequirementMiddleware(RequestDelegate next, string requiredModule)
    {
        this.next = next;
        this.requiredModule = requiredModule;
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

            if (modules is null || !modules.Contains(this.requiredModule, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Access to module '{this.requiredModule}' is forbidden.");
                return;
            }
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Invalid 'module' claim format.");
            return;
        }

        await this.next(context);
    }
}
