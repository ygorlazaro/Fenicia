using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.Data;

public class CompanyContext(IHttpContextAccessor http) : ICompanyContext
{
    private readonly IHttpContextAccessor http = http;

    public Guid? CompanyId
    {
        get
        {
            // First, try to get from JWT claim (authenticated requests)
            var claim = this.http.HttpContext?.User?
                .FindFirst("company_id");

            if (claim is not null && Guid.TryParse(claim.Value, out var claimCompanyId))
            {
                return claimCompanyId;
            }

            // Fallback to x-company header (for anonymous requests like login)
            if (this.http.HttpContext?.Request?.Headers?.TryGetValue("x-company", out var headerValue) == true)
            {
                if (Guid.TryParse(headerValue.ToString(), out var headerCompanyId))
                {
                    return headerCompanyId;
                }
            }

            return null;
        }
    }
}
