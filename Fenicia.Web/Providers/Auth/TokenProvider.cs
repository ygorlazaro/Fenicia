namespace Fenicia.Web.Providers.Auth;

using System.Net;

using Fenicia.Auth.Domains.RefreshToken.Data;
using Fenicia.Auth.Domains.Token.Data;

public class TokenProvider(HttpClient httpClient)
{
    public async Task<TokenResponse?> GetTokenAsync(TokenRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(requestUri: "Token", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TokenResponse>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException($"Login failed: {errorMessage}");
            }

            throw new HttpRequestException($"Server error ({response.StatusCode}): {errorMessage}");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Failed to get token: {ex.Message}", ex);
        }
    }

    public async Task<TokenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(requestUri: "Token/refresh", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TokenResponse>();
            }

            // Handle non-success status codes
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error: {response.StatusCode} - {errorMessage}");
        }
        catch (Exception ex)
        {
            // Log the exception if you have a logging mechanism
            throw new HttpRequestException(message: "Failed to refresh token", ex);
        }
    }
}
