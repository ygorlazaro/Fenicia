namespace Fenicia.Integration.RunCommandTool.Providers;

using Common.Database.Requests;
using Common.Database.Responses;

public class TokenProvider : BaseProvider
{
    public TokenProvider(string baseUrl)
        : base(baseUrl)
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

        return await PostAsync<TokenResponse, TokenRequest>("token", tokenRequest);
    }
}
