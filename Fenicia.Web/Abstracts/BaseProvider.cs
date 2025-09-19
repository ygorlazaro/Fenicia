using System.Net.Http.Headers;
using System.Text.Json;

public abstract class BaseProvider
{
    private readonly IConfiguration configuration;

    protected HttpClient httpClient;

    public BaseProvider(IConfiguration configuration)
    {

        this.configuration = configuration;
        this.httpClient = new HttpClient
        {
            BaseAddress = new Uri(this.configuration["Routes:BaseAuthUrl"] ?? throw new NullReferenceException())
        };
    }

    public async Task<TResponse> PostAsync<TResponse, TRequest>(string url, TRequest request)
    {
        using var content = new StringContent(JsonSerializer.Serialize(request));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await this.httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var userResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(responseContent);

            return JsonSerializer.Deserialize<TResponse>(responseContent) ?? throw new NullReferenceException();
        }
        else
        {
            throw new Exception("Request failed");
        }
    }
}
