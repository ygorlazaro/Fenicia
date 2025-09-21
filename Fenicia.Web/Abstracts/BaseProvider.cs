using System.Net.Http.Headers;
using System.Text.Json;

using Fenicia.Common;
using Fenicia.Web.Providers.Auth;

namespace Fenicia.Web.Abstracts;

public abstract class BaseProvider
{
    protected readonly HttpClient HttpClient;

    private readonly AuthManager authManager;

    protected BaseProvider(IConfiguration configuration, AuthManager authManager)
    {
        this.authManager = authManager;
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(configuration["Routes:BaseAuthUrl"] ?? throw new NullReferenceException())
        };
    }

    protected async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string route, TRequest request)
    {
        using var content = new StringContent(JsonSerializer.Serialize(request));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await HttpClient.PostAsync(route, content);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}). Response: {responseContent}");
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<ApiResponse<TResponse>>(responseContent, options) ?? throw new NullReferenceException();
    }

    protected async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string route)
    {
        var token = authManager.JwtToken;

        if (!string.IsNullOrEmpty(token))
        {
            HttpClient.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", token);
        }

        var response = await HttpClient.GetAsync(route);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}). Response: {responseContent}");
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<ApiResponse<TResponse>>(responseContent, options) ?? throw new NullReferenceException();
    }
}
