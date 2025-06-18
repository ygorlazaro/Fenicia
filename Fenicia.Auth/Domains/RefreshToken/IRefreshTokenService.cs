using Fenicia.Common;

namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenService
{
    Task<ApiResponse<string>> GenerateRefreshTokenAsync(Guid userId);
    Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken);
    Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken);
}
