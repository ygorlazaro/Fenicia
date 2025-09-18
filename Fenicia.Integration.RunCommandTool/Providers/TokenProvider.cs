namespace Fenicia.Integration.RunCommandTool.Providers;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

public class TokenProvider : BaseProvider
{
    public TokenProvider(string baseUrl)
        : base(baseUrl)
    {
    }

    public async Task<TokenResponse> DoLoginAsync(string email, string password, string cnpj)
    {
        var tokenRequest = new TokenRequest()
        {
            Email = email,
            Cnpj = cnpj,
            Password = password
        };

        return await this.PostAsync<TokenResponse, TokenRequest>("token", tokenRequest);
    }
}
