using Fenicia.Common.Api.Providers;

using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.Api.Middlewares;

public class TenantMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

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
