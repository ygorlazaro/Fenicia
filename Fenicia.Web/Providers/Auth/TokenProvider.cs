using System.Net.Http.Headers;

using Blazored.LocalStorage;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Web.Providers.Auth;

public class TokenProvider : BaseProvider
{
    private readonly ILocalStorageService localStorage;
    private readonly ApiAuthenticationStateProvider apiAuthenticationStateProvider;

    public TokenProvider(IConfiguration configuration, ILocalStorageService localStorage, ApiAuthenticationStateProvider apiAuthenticationStateProvider) : base(configuration)
    {
        this.localStorage = localStorage;
        this.apiAuthenticationStateProvider = apiAuthenticationStateProvider;
    }

    public async Task<TokenResponse> DoLoginAsync(TokenRequest request)
    {
        var token = await PostAsync<TokenResponse, TokenRequest>("token", request);

        await this.localStorage.SetItemAsync("authToken", token.AccessToken);
        apiAuthenticationStateProvider.MarkUserAsAuthenticated(request.Email);

        base.httpClient.DefaultRequestHeaders.Authorization = new
            AuthenticationHeaderValue("Bearer", token.AccessToken);

        return token;
    }
}
