namespace Fenicia.Common.API.Middlewares;

using Microsoft.AspNetCore.Http;

using Providers;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task Invoke(HttpContext context, TenantProvider tenantProvider)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantId = context.User.FindFirst("companyId")?.Value;
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                tenantProvider.SetTenant(tenantId);
            }
        }

        await _next(context);
    }
}
