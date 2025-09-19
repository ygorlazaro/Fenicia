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

    protected async Task<TResponse> PostAsync<TResponse, TRequest>(string url, TRequest request)
    {
        using var content = new StringContent(JsonSerializer.Serialize(request));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await HttpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Request failed");
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<TResponse>(responseContent) ?? throw new NullReferenceException();

    }
}
