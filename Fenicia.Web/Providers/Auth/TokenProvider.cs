using System.Net.Http.Headers;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Web.Abstracts;


namespace Fenicia.Web.Providers.Auth;

public class TokenProvider : BaseProvider
{
    private readonly AuthManager authManager;

    public TokenProvider(IConfiguration configuration, AuthManager authManager) : base(configuration)
    {
        this.authManager = authManager;
    }

    public async Task<TokenResponse> DoLoginAsync(TokenRequest request)
    {
        var token = await PostAsync<TokenResponse, TokenRequest>("token", request);

        authManager.SetToken(token.AccessToken);

        HttpClient.DefaultRequestHeaders.Authorization = new
        AuthenticationHeaderValue("Bearer", token.AccessToken);

        return token;
    }
}
