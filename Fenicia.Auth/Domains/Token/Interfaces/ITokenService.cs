using Fenicia.Common.DTOs.Auth.Token;

namespace Fenicia.Auth.Domains.Token.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateAsync(TokenRequest request, CancellationToken cancellationToken = default);

    string GenerateString(TokenResponse user);
}