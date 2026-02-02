using Fenicia.Common;

namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenService
{
    ApiResponse<string> GenerateRefreshToken(Guid userId);

    Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);

    Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
