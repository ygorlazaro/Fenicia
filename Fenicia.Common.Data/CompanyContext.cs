using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.Data;

public class CompanyContext(IHttpContextAccessor http) : ICompanyContext
{
    public Guid CompanyId
    {
        get
        {
            var claim = http.HttpContext?.User?.FindFirst("company_id");

            return claim is not null && Guid.TryParse(claim.Value, out var claimCompanyId)
                ? claimCompanyId
                : http.HttpContext?.Request?.Headers?.TryGetValue("x-company", out var headerValue) != true
                ? Guid.Empty
                : !Guid.TryParse(headerValue.ToString(), out var headerCompanyId) ? Guid.Empty : headerCompanyId;
        }
    }
}
