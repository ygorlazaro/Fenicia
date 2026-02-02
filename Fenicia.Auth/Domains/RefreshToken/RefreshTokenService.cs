using System.Security.Cryptography;

using Fenicia.Common;

namespace Fenicia.Auth.Domains.RefreshToken;

public sealed class RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
{
    public ApiResponse<string> GenerateRefreshToken(Guid userId)
    {
        try
        {
            logger.LogInformation("Starting refresh token generation for user {UserID}", userId);
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                UserId = userId,
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                IsActive = true
            };

            refreshTokenRepository.Add(refreshToken);

            logger.LogInformation("Successfully generated refresh token for user {UserID}", userId);

            return new ApiResponse<string>(refreshToken.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating refresh token for user {UserID}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Validating refresh token for user {UserID}", userId);

            var response = await refreshTokenRepository.ValidateTokenAsync(userId, refreshToken, cancellationToken);

            logger.LogInformation("Token validation result for user {UserID}: {IsValid}", userId, response);

            return new ApiResponse<bool>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating refresh token for user {UserID}", userId);

            throw;
        }
    }

    public async Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Invalidating refresh token");

            await refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken, cancellationToken);

            logger.LogInformation("Successfully invalidated refresh token");

            return new ApiResponse<object>(data: null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error invalidating refresh token");

            throw;
        }
    }
}
