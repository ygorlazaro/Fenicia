using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Fenicia.Integration.RunCommandTool;

public abstract class BaseProvider(string uri)
{
    private readonly HttpClient client = new()
    {
        BaseAddress = new Uri(uri)
    };

    protected BaseProvider(string uri, string accessToken)
        : this(uri)
    {
        SetToken(accessToken);
    }

    public void SetToken(string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    protected async Task<TResponse> PostAsync<TResponse, TRequest>(string route, TRequest request)
    {
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(route, content);
        var deserialized = await response.Content.ReadFromJsonAsync<TResponse>();

        ArgumentNullException.ThrowIfNull(deserialized);

        return deserialized;
    }

    protected async Task<T> GetAsync<T>(string route)
    {
        var response = await client.GetAsync(route);
        var data = await response.Content.ReadFromJsonAsync<T>();

        ArgumentNullException.ThrowIfNull(data);

        return data;
    }
}
