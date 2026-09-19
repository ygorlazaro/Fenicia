using System.Security.Cryptography;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Common.DTOs.Auth.RefreshToken;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.RefreshToken;

public sealed class RefreshTokenService(RefreshTokenMapper mapper, IRefreshTokenRepository repository) : IRefreshTokenService
{
    public async Task<string> GenerateAsync(Guid userId)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var stringToken = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshTokenModel(stringToken, DateTime.UtcNow.AddDays(7), userId);

        await repository.AddAsync(refreshToken);

        var mapped = mapper.MapToRefreshTokenResponse(refreshToken);

        return mapped.Token;
    }

    public async Task<RefreshTokenResponse?> GetAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var tokenModel = await repository.GetAsync(token);

        return tokenModel is null ? null : mapper.MapToRefreshTokenResponse(tokenModel);
    }


    public async Task<bool> ValidateAsync(
        Guid userId,
        string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        var token = await repository.GetAsync(refreshToken);

        return token != null && token.UserId == userId && token.IsActive && token.ExpirationDate > DateTime.UtcNow;
    }
}