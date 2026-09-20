using Fenicia.Common.DTOs.Auth.RefreshToken;

namespace Fenicia.Auth.Domains.RefreshToken.Interfaces;

public interface IRefreshTokenService
{
    Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<RefreshTokenResponse?> GetAsync(string token, CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
}
