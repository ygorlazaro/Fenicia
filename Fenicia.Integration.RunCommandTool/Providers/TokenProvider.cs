using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Integration.RunCommandTool.Providers;

public class TokenProvider(string uri) : BaseProvider(uri)
{
    public async Task<TokenResponse> DoLoginAsync(string email, string password)
    {
        var tokenRequest = new TokenRequest
                           {
            Email = email,
            Password = password
                           };

        return await this.PostAsync<TokenResponse, TokenRequest>("token", tokenRequest);
    }
}
