namespace Fenicia.Web;

public class CompanyHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private const string _headerName = "CompanyId";
    private const string _cookieName = "selected_company_id";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Guid? companyId = null;

        var cookie = httpContextAccessor.HttpContext?.Request.Cookies[_cookieName];
        if (!string.IsNullOrEmpty(cookie) && Guid.TryParse(cookie, out var fromCookie))
        {
            companyId = fromCookie;
        }

        if (!companyId.HasValue)
        {
            return base.SendAsync(request, cancellationToken);
        }

        if (request.Headers.Contains(_headerName))
        {
            request.Headers.Remove(_headerName);
        }

        request.Headers.Add(_headerName, companyId.Value.ToString());

        return base.SendAsync(request, cancellationToken);
    }
}
