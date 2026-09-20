using System.Security.Cryptography;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Common.DTOs.Auth.RefreshToken;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.RefreshToken;

public sealed class RefreshTokenService(IRefreshTokenRepository repository) : IRefreshTokenService
{
    public async Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var stringToken = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshTokenModel(stringToken, DateTime.UtcNow.AddDays(7), userId);

        await repository.AddAsync(refreshToken);

        return stringToken;
    }

    public async Task<RefreshTokenResponse?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var tokenModel = await repository.GetAsync(token);

        return tokenModel is null ? null : MapToRefreshTokenResponse(tokenModel);
    }

    public async Task<bool> ValidateAsync(
        Guid userId,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        var token = await repository.GetAsync(refreshToken);

        return token != null && token.UserId == userId && token.IsActive && token.ExpirationDate > DateTime.UtcNow;
    }

    private static RefreshTokenResponse MapToRefreshTokenResponse(RefreshTokenModel token)
    {
        return new RefreshTokenResponse(
            token.Token,
            token.ExpirationDate,
            token.UserId,
            token.IsActive);
    }
}
