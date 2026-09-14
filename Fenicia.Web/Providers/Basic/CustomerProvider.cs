using System.Text.Json;
using Fenicia.Common.DTOs.Basic.State;
using Fenicia.Web.Services;
using Fenicia.Web.Services.Interfaces;

namespace Fenicia.Web.Providers.Basic;

public class CustomerProvider(IHttpClientFactory httpClientFactory, ICompanyContextService companyContext)
    : BasicProvider(httpClientFactory, companyContext)
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<GetAllStateResponse>> GetStatesAsync(CancellationToken ct = default)
    {
        var client = await BuildBasicClientAsync();
        var response = await client.GetAsync("state", ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new CrudApiException($"Falha ao carregar estados: {(int)response.StatusCode} {response.StatusCode} - {body}", response.StatusCode);
        }

        return JsonSerializer.Deserialize<List<GetAllStateResponse>>(body, _jsonOptions) ?? [];
    }
}
