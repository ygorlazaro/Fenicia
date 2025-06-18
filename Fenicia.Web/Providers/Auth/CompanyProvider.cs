using Fenicia.Common;
using System.Net.Http.Json;

using Fenicia.Auth.Domains.Company;

namespace Fenicia.Web.Providers.Auth;

public class CompanyProvider
{
    private readonly HttpClient _httpClient;

    public CompanyProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CompanyResponse>?> GetCompaniesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("company");
            if (response.IsSuccessStatusCode)
            {
                var pagination = await response.Content.ReadFromJsonAsync<Pagination<IEnumerable<CompanyResponse>>>();
                return pagination?.Data;
            }
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to get companies: {errorMessage}");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Failed to get companies: {ex.Message}", ex);
        }
    }
}
