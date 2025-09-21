namespace Fenicia.Auth.Domains.RefreshToken;

using Common;

public interface IRefreshTokenService
{
    ApiResponse<string> GenerateRefreshToken(Guid userId);

    Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);

    Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
