using Fenicia.Common.DTOs.Auth.RefreshToken;

namespace Fenicia.Auth.Domains.RefreshToken.Interfaces;

public interface IRefreshTokenService
{
    Task<string> GenerateAsync(Guid userId);

    Task<ValidateTokenResponse?> GetAsync(string token);

    Task<RefreshTokenModel> UpdateAsync(string token, bool isActive);

    Task<bool> ValidateAsync(Guid userId, string refreshToken);
}