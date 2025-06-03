using System.Security.Cryptography;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class RefreshTokenService(ILogger<RefreshTokenService> logger, IRefreshTokenRepository refreshTokenRepository): IRefreshTokenService
{
    public async Task<string> GenerateRefreshTokenAsync(Guid userId)
    {
        logger.LogInformation("Generating refresh token");
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var refreshToken = new RefreshTokenModel()
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = userId
        };

        refreshTokenRepository.Add(refreshToken);

        await refreshTokenRepository.SaveChangesAsync();

        return refreshToken.Token;
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken)
    {
        return await refreshTokenRepository.ValidateTokenAsync(userId, refreshToken);
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken)
    {
        await refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken);
    }
}