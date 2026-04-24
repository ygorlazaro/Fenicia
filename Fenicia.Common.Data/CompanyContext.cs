using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.Data;

public class CompanyContext(IHttpContextAccessor http) : ICompanyContext
{
    public Guid CompanyId
    {
        get
        {
            // Try JWT claims first (supports both old and new claim names)
            var claim = http.HttpContext?.User?.FindFirst("company_id")
                     ?? http.HttpContext?.User?.FindFirst("companyId");

            if (claim is not null && Guid.TryParse(claim.Value, out var claimCompanyId))
            {
                return claimCompanyId;
            }

            // Fallback to HTTP header when JWT claim is not present
            // (e.g., after login but before company selection is encoded in token)
            var headerValue = http.HttpContext?.Request?.Headers["CompanyId"].FirstOrDefault()
                           ?? http.HttpContext?.Request?.Headers["companyId"].FirstOrDefault();

            if (!string.IsNullOrEmpty(headerValue) && Guid.TryParse(headerValue, out var headerCompanyId))
            {
                return headerCompanyId;
            }

            return Guid.Empty;
        }
    }
}
