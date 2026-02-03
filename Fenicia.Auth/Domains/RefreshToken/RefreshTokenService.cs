using System.Security.Cryptography;

using Fenicia.Common;

namespace Fenicia.Auth.Domains.RefreshToken;

public sealed class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
{
    public ApiResponse<string> GenerateRefreshToken(Guid userId)
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

        return new ApiResponse<string>(refreshToken.Token);
    }

    public async Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        var response = await refreshTokenRepository.ValidateTokenAsync(userId, refreshToken, cancellationToken);

        return new ApiResponse<bool>(response);
    }

    public async Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        await refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken, cancellationToken);

        return new ApiResponse<object>(data: null);
    }
}
