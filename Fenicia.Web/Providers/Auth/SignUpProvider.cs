namespace Fenicia.Web.Providers.Auth;

using System.Net;

using Fenicia.Auth.Domains.User.Data;

public class SignUpProvider(HttpClient httpClient)
{
    public async Task<UserResponse?> CreateNewUserAsync(UserRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(requestUri: "SignUp", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserResponse>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException($"Sign up failed: {errorMessage}");
            }

            throw new HttpRequestException($"Server error ({response.StatusCode}): {errorMessage}");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Failed to sign up: {ex.Message}", ex);
        }
    }
}
