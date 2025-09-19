namespace Fenicia.Integration.RunCommandTool;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public abstract class BaseProvider
{
    private readonly HttpClient _client;

    protected BaseProvider(string baseUrl)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    protected BaseProvider(string baseUrl, string accessToken)
        : this(baseUrl)
    {
        SetToken(accessToken);
    }

    public void SetToken(string accessToken)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    protected async Task<TResponse> PostAsync<TResponse, TRequest>(string route, TRequest request)
    {
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8,
                                      "application/json");

        var response = await _client.PostAsync(route, content);

        var deserialized = await response.Content.ReadFromJsonAsync<TResponse>();

        ArgumentNullException.ThrowIfNull(deserialized);

        return deserialized;
    }

    protected async Task<T> GetAsync<T>(string route)
    {
        var response = await _client.GetAsync(route);

        var data = await response.Content.ReadFromJsonAsync<T>();

        ArgumentNullException.ThrowIfNull(data);

        return data;
    }
}
