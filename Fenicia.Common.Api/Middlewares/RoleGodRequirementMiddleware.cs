namespace Fenicia.Common.Api.Middlewares;

using System.Text.Json;

using Microsoft.AspNetCore.Http;

public class RoleGodRequirementMiddleware
{
    private readonly RequestDelegate _next;

    public RoleGodRequirementMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var roleClaim = context.User.FindFirst("role");

        if (roleClaim == null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Missing 'role' claim.");
            return;
        }

        try
        {
            var roles = JsonSerializer.Deserialize<List<string>>(roleClaim.Value);
            var requiredRole = "Admin";

            if (roles is null || !roles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Access to role'{requiredRole}' is forbidden.");
                return;
            }
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Invalid 'role' claim format.");
            return;
        }

        await _next(context);
    }
}
