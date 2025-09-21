namespace Fenicia.Integration.RunCommandTool.Providers;

using Common.Database.Requests;
using Common.Database.Responses;

public class TokenProvider : BaseProvider
{
    public TokenProvider(string uri)
        : base(uri)
    {
    }

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
