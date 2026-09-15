using System.Security.Cryptography;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Common.DTOs.Auth.RefreshToken;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.RefreshToken;

public sealed class RefreshTokenService(IRefreshTokenRepository repository, RefreshTokenMapper refreshTokenMapper) : IRefreshTokenService
{
    public async Task<string> GenerateAsync(Guid userId)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var stringToken = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshTokenModel(stringToken, DateTime.UtcNow.AddDays(7), userId);

        await repository.AddAsync(refreshToken);

        var mapped = refreshTokenMapper.MapToGenerateRefreshTokenResponse(refreshToken);

        return mapped.Token;
    }

    public async Task<ValidateTokenResponse?> GetAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var tokenModel = await repository.GetAsync(token);

        return tokenModel is null ? null : refreshTokenMapper.MapToValidateTokenResponse(tokenModel);
    }

    public async Task<RefreshTokenModel> UpdateAsync(
        string token,
        bool isActive)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        var existing = await repository.GetAsync(token);

        if (existing is null)
        {
            throw new ItemNotExistsException(ExceptionMessages.ItemNotFound);
        }

        existing.IsActive = isActive;

        await repository.UpdateAsync(existing);

        return existing;
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