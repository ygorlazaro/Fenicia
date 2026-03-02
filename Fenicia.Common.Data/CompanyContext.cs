using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.Data;

public class CompanyContext(IHttpContextAccessor http) : ICompanyContext
{
    private readonly IHttpContextAccessor http = http;

    public Guid? CompanyId
    {
        get
        {
            var claim = this.http.HttpContext?.User?
                .FindFirst("company_id");

            return claim is null
                ? null
                : Guid.Parse(claim.Value);
        }
    }
}
