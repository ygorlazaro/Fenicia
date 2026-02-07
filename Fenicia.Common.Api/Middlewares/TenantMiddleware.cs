using Fenicia.Common.API.Providers;

using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.API.Middlewares;

public class TenantMiddleware(RequestDelegate next)
{
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

        await next(context);
    }
}