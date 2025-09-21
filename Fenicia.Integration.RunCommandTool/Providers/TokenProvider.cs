namespace Fenicia.Integration.RunCommandTool.Providers;

using Common.Database.Requests;
using Common.Database.Responses;

public class TokenProvider : BaseProvider
{
    public TokenProvider(Uri uri)
        : base(uri)
    {
    }

    public async Task<TokenResponse> DoLoginAsync(string email, string password, string cnpj)
    {
        var tokenRequest = new TokenRequest
                           {
            Email = email,
            Cnpj = cnpj,
            Password = password
                           };

        return await this.PostAsync<TokenResponse, TokenRequest>("token", tokenRequest);
    }
}
