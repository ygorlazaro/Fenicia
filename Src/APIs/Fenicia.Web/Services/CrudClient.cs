using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Fenicia.Common;
using MudBlazor;

namespace Fenicia.Web.Services;

public interface ICrudClient
{
    Task<TableData<TItem>> GetPageAsync<TItem>(string endpoint, TableState state, CancellationToken ct);

    Task<HttpResponseMessage> PostAsync<TPayload>(string endpoint, TPayload payload, CancellationToken ct);

    Task<HttpResponseMessage> PatchAsync<TPayload>(string endpoint, Guid id, TPayload payload, CancellationToken ct);

    Task<HttpResponseMessage> DeleteAsync(string endpoint, Guid id, CancellationToken ct);
}

public class CrudClient(IHttpClientFactory httpClientFactory, ICompanyContextService companyContext) : ICrudClient
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<TableData<TItem>> GetPageAsync<TItem>(string endpoint, TableState state, CancellationToken ct)
    {
        var client = await CreateClientAsync();

        var page = state.Page + 1;
        var perPage = state.PageSize;
        var sort = state.SortLabel;
        var direction = state.SortDirection == SortDirection.Descending ? "desc" : "asc";

        var query = $"?page={page}&perPage={perPage}";
        if (!string.IsNullOrWhiteSpace(sort))
        {
            query += $"&sort={Uri.EscapeDataString(sort)}&direction={direction}";
        }

        var response = await client.GetAsync($"{endpoint}{query}", ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new CrudApiException($"Falha ao carregar: {(int)response.StatusCode} {response.StatusCode} - {body}", response.StatusCode);
        }

        var paginated = JsonSerializer.Deserialize<Pagination<List<TItem>>>(body, _jsonOptions);
        return new TableData<TItem>
        {
            TotalItems = paginated?.Total ?? 0,
            Items = paginated?.Data ?? []
        };
    }

    public async Task<HttpResponseMessage> PostAsync<TPayload>(string endpoint, TPayload payload, CancellationToken ct)
    {
        var client = await CreateClientAsync();
        var response = await client.PostAsJsonAsync(endpoint, payload, _jsonOptions, ct);
        await EnsureSuccessAsync(response, "criar");
        return response;
    }

    public async Task<HttpResponseMessage> PatchAsync<TPayload>(string endpoint, Guid id, TPayload payload, CancellationToken ct)
    {
        var client = await CreateClientAsync();
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{endpoint}/{id}") { Content = content };
        var response = await client.SendAsync(request, ct);
        await EnsureSuccessAsync(response, "atualizar");
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint, Guid id, CancellationToken ct)
    {
        var client = await CreateClientAsync();
        var response = await client.DeleteAsync($"{endpoint}/{id}", ct);
        await EnsureSuccessAsync(response, "excluir");
        return response;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string operation)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        throw new CrudApiException($"Falha ao {operation}: {(int)response.StatusCode} {response.StatusCode} - {body}", response.StatusCode);
    }

    private async Task<HttpClient> CreateClientAsync()
    {
        var client = httpClientFactory.CreateClient("FeniciaBasic");

        var token = await companyContext.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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
