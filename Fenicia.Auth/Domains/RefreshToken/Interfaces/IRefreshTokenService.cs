using Fenicia.Common.DTOs.Auth.RefreshToken;

namespace Fenicia.Auth.Domains.RefreshToken.Interfaces;

public interface IRefreshTokenService
{
    Task<string> GenerateAsync(Guid userId);

    Task<RefreshTokenResponse?> GetAsync(string token);

    Task<bool> ValidateAsync(Guid userId, string refreshToken);
}