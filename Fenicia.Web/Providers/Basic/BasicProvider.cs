using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Fenicia.Web.Services.Interfaces;
using Microsoft.Extensions.Http;

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
