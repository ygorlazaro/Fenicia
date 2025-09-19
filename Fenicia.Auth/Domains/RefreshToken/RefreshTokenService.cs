namespace Fenicia.Auth.Domains.RefreshToken;

using System.Security.Cryptography;

using Common;
using Common.Database.Models.Auth;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly ILogger<RefreshTokenService> logger;
    private readonly IRefreshTokenRepository refreshTokenRepository;

    public RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository)
    {
        this.logger = logger;
        this.refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse<string>> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting refresh token generation for user {UserID}", userId);
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
