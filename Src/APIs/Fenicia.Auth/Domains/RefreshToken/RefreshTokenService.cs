using System.Security.Cryptography;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
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

        await repository.AddAsync(refreshToken, cancellationToken);

        return refreshToken.Token;
    }

    public async Task<RefreshTokenModel?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        return string.IsNullOrWhiteSpace(token) ? null : await repository.GetAsync(token, cancellationToken);
    }

    public async Task<RefreshTokenModel> UpdateAsync(
        string token,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        var existing = await repository.GetAsync(token, cancellationToken);

        if (existing is null)
        {
            throw new ItemNotExistsException(ExceptionMessages.ItemNotFound);
        }

        var updated = existing with { IsActive = isActive };
        await repository.UpdateAsync(updated, cancellationToken);

        return updated;
    }

    public async Task InvalidateAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var token = await repository.GetAsync(refreshToken, cancellationToken);

        if (token is null)
        {
            return;
        }

        var updatedToken = token with { IsActive = false };
        await repository.UpdateAsync(updatedToken, cancellationToken);
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

        var token = await repository.GetAsync(refreshToken, cancellationToken);

        return token != null && token.UserId == userId && token.IsActive && token.ExpirationDate > DateTime.UtcNow;
    }
}