namespace Fenicia.Integration.RunCommandTool;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public abstract class BaseProvider
{
    private readonly HttpClient client;

    protected BaseProvider(Uri uri)
    {
        this.client = new HttpClient
        {
            BaseAddress = uri
        };
    }

    protected BaseProvider(Uri uri, string accessToken)
        : this(uri)
    {
        this.SetToken(accessToken);
    }

    public void SetToken(string accessToken)
    {
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    protected async Task<TResponse> PostAsync<TResponse, TRequest>(string route, TRequest request)
    {
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8,
                                      "application/json");

        var response = await this.client.PostAsync(route, content);

        var deserialized = await response.Content.ReadFromJsonAsync<TResponse>();

        ArgumentNullException.ThrowIfNull(deserialized);

        return deserialized;
    }

    protected async Task<T> GetAsync<T>(string route)
    {
        var response = await this.client.GetAsync(route);

        var data = await response.Content.ReadFromJsonAsync<T>();

        ArgumentNullException.ThrowIfNull(data);

        return data;
    }
}
