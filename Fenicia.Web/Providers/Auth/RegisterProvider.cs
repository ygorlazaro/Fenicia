using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

using System.Net.Http.Headers;
using System.Text.Json;

namespace Fenicia.Web.Providers.Auth;

public class RegisterProvider
{

    private readonly AuthManager authManager;
    private readonly HttpClient httpClient;

    public RegisterProvider(IConfiguration configuration, AuthManager authManager, HttpClient httpClient)
    {
        this.authManager = authManager;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri(configuration["Routes:]BaseAuthUrl"] ?? throw new NullReferenceException());
    }

    public async Task<UserResponse> RegisterAsync(UserRequest request)
    {
        using var content = new StringContent(JsonSerializer.Serialize(request));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await httpClient.PostAsync("register", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}). Response: {responseContent}");
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<UserResponse>(responseContent, options) ?? throw new NullReferenceException();
    }
}
