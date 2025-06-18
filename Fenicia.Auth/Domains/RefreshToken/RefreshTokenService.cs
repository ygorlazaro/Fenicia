using System.Security.Cryptography;

using Fenicia.Common;

namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshTokenService(
    ILogger<RefreshTokenService> logger,
    IRefreshTokenRepository refreshTokenRepository
) : IRefreshTokenService
{
    public async Task<ApiResponse<string>> GenerateRefreshTokenAsync(Guid userId)
    {
        logger.LogInformation("Generating refresh token");
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var refreshToken = new RefreshTokenModel
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = userId,
        };

        refreshTokenRepository.Add(refreshToken);

        await refreshTokenRepository.SaveChangesAsync();

        return new ApiResponse<string>(refreshToken.Token);
    }

    public async Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken)
    {
        var response = await refreshTokenRepository.ValidateTokenAsync(userId, refreshToken);

        return new ApiResponse<bool>(response);
    }

    public async Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken)
    {
        await refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken);

        return new ApiResponse<object>(null);
    }
}
