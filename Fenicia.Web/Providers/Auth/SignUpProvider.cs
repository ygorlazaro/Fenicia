using Fenicia.Auth.Domains.User;

namespace Fenicia.Web.Providers.Auth;

public class SignUpProvider(HttpClient httpClient)
{
    public async Task<UserResponse?> CreateNewUserAsync(UserRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("SignUp", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserResponse>();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
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
