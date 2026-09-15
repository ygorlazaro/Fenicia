using System.Net.Http.Headers;
using Fenicia.Web.Services.Interfaces;

namespace Fenicia.Web.Providers.Basic;

public class BasicProvider(IHttpClientFactory httpClientFactory, ICompanyContextService companyContext)
{
    protected async Task<HttpClient> BuildBasicClientAsync(CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("FeniciaBasic");

        var token = await companyContext.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var companyId = await companyContext.GetSelectedCompanyIdAsync();
        if (companyId.HasValue)
        {
            client.DefaultRequestHeaders.Remove("CompanyId");
            client.DefaultRequestHeaders.Add("CompanyId", companyId.Value.ToString());
        }

        return client;
    }
}
