using Fenicia.Auth.Domains.Token.DTOs;

namespace Fenicia.Auth.Domains.Token.Interfaces;

public interface ITokenService
{
    Task<GenerateTokenResponse> GenerateAsync(GenerateTokenQuery query, CancellationToken cancellationToken = default);

    string GenerateString(GenerateTokenResponse user);
}