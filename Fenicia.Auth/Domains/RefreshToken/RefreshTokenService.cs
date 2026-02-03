using System.Security.Cryptography;

namespace Fenicia.Auth.Domains.RefreshToken;

public sealed class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
{
    public string GenerateRefreshToken(Guid userId)
    {
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

        return refreshToken.Token;
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        return await refreshTokenRepository.ValidateTokenAsync(userId, refreshToken, cancellationToken);
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        await refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken, cancellationToken);
    }
}
