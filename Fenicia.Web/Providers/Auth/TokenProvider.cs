using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

using System.Net.Http.Headers;
using System.Text.Json;

namespace Fenicia.Web.Providers.Auth;

public class TokenProvider
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public TokenProvider(IConfiguration configuration)
    {

        _configuration = configuration;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_configuration["Routes:BaseAuthUrl"] ?? throw new NullReferenceException())
        };
    }

    public async Task<TokenResponse> DoLoginAsync(TokenRequest request)
    {
        using var content = new StringContent(JsonSerializer.Serialize(request));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("/token", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var userResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserResponse>(responseContent);

            return JsonSerializer.Deserialize<TokenResponse>(responseContent) ?? throw new NullReferenceException();
        }
        else
        {
            throw new Exception($"Failed to login. Status code: {response.StatusCode}");
        }
    }
}
