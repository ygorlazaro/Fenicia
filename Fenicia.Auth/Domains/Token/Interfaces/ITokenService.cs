using Fenicia.Common.DTOs.Auth.Token;

namespace Fenicia.Auth.Domains.Token.Interfaces;

public interface ITokenService
{
    Task<GenerateTokenResponse> GenerateAsync(GenerateTokenQuery query, CancellationToken cancellationToken = default);

    string GenerateString(GenerateTokenResponse user);
}