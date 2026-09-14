using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.Data;

public class CompanyContext(IHttpContextAccessor http) : ICompanyContext
{
    public Guid CompanyId
    {
        get
        {
            var jwtCompanyId = GetJwtCompanyId();
            var headerCompanyId = GetHeaderCompanyId();

            if (jwtCompanyId.HasValue && headerCompanyId.HasValue)
            {
                return jwtCompanyId.Value != headerCompanyId.Value
                    ? throw new InvalidOperationException("CompanyId mismatch between JWT claim and HTTP header.")
                    : jwtCompanyId.Value;
            }

            if (jwtCompanyId.HasValue)
            {
                return jwtCompanyId.Value;
            }

            return headerCompanyId ?? Guid.Empty;
        }
    }

    private Guid? GetJwtCompanyId()
    {
        var claim = http.HttpContext?.User?.FindFirst("company_id")
                    ?? http.HttpContext?.User?.FindFirst("companyId");

        return claim is not null && Guid.TryParse(claim.Value, out var claimCompanyId) ? claimCompanyId : null;
    }

    private Guid? GetHeaderCompanyId()
    {
        var headerValue = http.HttpContext?.Request?.Headers["CompanyId"].FirstOrDefault()
                          ?? http.HttpContext?.Request?.Headers["companyId"].FirstOrDefault();

        return !string.IsNullOrEmpty(headerValue) && Guid.TryParse(headerValue, out var headerCompanyId)
            ? headerCompanyId
            : null;
    }
}