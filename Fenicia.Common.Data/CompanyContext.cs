using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.Data;

public class CompanyContext(IHttpContextAccessor http) : ICompanyContext
{
    public Guid CompanyId
    {
        get
        {
            var claim = http.HttpContext?.User?.FindFirst("company_id");

            if (claim is not null && Guid.TryParse(claim.Value, out var claimCompanyId))
            {
                return claimCompanyId;
            }

            if (http.HttpContext?.Request?.Headers?.TryGetValue("x-company", out var headerValue) != true)
            {
                return Guid.Empty;
            }

            if (!Guid.TryParse(headerValue.ToString(), out var headerCompanyId))
            {
                return Guid.Empty;
            }

            return headerCompanyId;

        }
    }
}