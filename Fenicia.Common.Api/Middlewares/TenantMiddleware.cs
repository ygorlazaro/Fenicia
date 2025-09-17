namespace Fenicia.Common.API.Middlewares;

using Microsoft.AspNetCore.Http;

using Providers;

public class TenantMiddleware
{
    private readonly RequestDelegate next;

    public TenantMiddleware(RequestDelegate next)
    {
        this.next = next;
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

        await this.next(context);
    }
}
