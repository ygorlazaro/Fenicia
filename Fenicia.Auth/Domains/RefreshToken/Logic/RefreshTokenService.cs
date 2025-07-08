namespace Fenicia.Auth.Domains.RefreshToken.Logic;

using System.Security.Cryptography;

using Common;

using Data;

public sealed class RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
{
    public async Task<ApiResponse<string>> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Starting refresh token generation for user {UserId}", userId);
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = new RefreshTokenModel
            {
                Token = Convert.ToBase64String(randomNumber),
                UserId = userId
            };

            refreshTokenRepository.Add(refreshToken);

            await refreshTokenRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation(message: "Successfully generated refresh token for user {UserId}", userId);
            return new ApiResponse<string>(refreshToken.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error generating refresh token for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Validating refresh token for user {UserId}", userId);
            var response = await refreshTokenRepository.ValidateTokenAsync(userId, refreshToken, cancellationToken);

            logger.LogInformation(message: "Token validation result for user {UserId}: {IsValid}", userId, response);
            return new ApiResponse<bool>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error validating refresh token for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Invalidating refresh token");
            await refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken, cancellationToken);

            logger.LogInformation(message: "Successfully invalidated refresh token");
            return new ApiResponse<object>(data: null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error invalidating refresh token");
            throw;
        }
    }
}
