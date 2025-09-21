using System.Net.Http.Headers;
using System.Text.Json;

namespace Fenicia.Web.Abstracts;

public abstract class BaseProvider
{
    protected readonly HttpClient HttpClient;

    protected BaseProvider(IConfiguration configuration)
    {
        HttpClient = new HttpClient
                     {
                         BaseAddress = new Uri(configuration["Routes:BaseAuthUrl"] ?? throw new NullReferenceException())
                     };
    }

    protected async Task<TResponse> PostAsync<TResponse, TRequest>(string route, TRequest request)
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
        return JsonSerializer.Deserialize<TResponse>(responseContent, options) ?? throw new NullReferenceException();

    }
}
