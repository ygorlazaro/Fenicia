using System.Security.Cryptography;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class RefreshTokenService(
    ILogger<RefreshTokenService> logger,
    IRefreshTokenRepository refreshTokenRepository
) : IRefreshTokenService
{
    public async Task<ServiceResponse<string>> GenerateRefreshTokenAsync(Guid userId)
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

        return new ServiceResponse<string>(refreshToken.Token);
    }

    public async Task<ServiceResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken)
    {
        var response = await refreshTokenRepository.ValidateTokenAsync(userId, refreshToken);

        return new ServiceResponse<bool>(response);
    }

    public async Task<ServiceResponse<object>> InvalidateRefreshTokenAsync(string refreshToken)
    {
        await refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken);

        return new ServiceResponse<object>(null);
    }
}
