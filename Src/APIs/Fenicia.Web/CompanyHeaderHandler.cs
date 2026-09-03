namespace Fenicia.Web;

public class CompanyHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private const string HeaderName = "CompanyId";
    private const string CookieName = "selected_company_id";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Guid? companyId = null;

        var cookie = httpContextAccessor.HttpContext?.Request.Cookies[CookieName];
        if (!string.IsNullOrEmpty(cookie) && Guid.TryParse(cookie, out var fromCookie))
        {
            companyId = fromCookie;
        }

        if (companyId.HasValue)
        {
            if (request.Headers.Contains(HeaderName))
            {
                request.Headers.Remove(HeaderName);
            }

            request.Headers.Add(HeaderName, companyId.Value.ToString());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
