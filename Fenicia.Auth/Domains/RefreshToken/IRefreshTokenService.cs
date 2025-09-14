namespace Fenicia.Auth.Domains.RefreshToken;

using Common;

public interface IRefreshTokenService
{
    Task<ApiResponse<string>> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);
    Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
