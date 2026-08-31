using System.Security.Cryptography;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshTokenService(IRefreshTokenRepository repository)
{
    public virtual async Task<string> GenerateAsync(Guid userId, CancellationToken ct)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var stringToken = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshTokenModel(stringToken, DateTime.UtcNow.AddDays(7), userId);

        await repository.AddAsync(refreshToken, ct);

        return refreshToken.Token;
    }

    public virtual async Task<RefreshTokenModel?> GetAsync(string token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return await repository.GetAsync(token, ct);
    }

    public virtual async Task<RefreshTokenModel> UpdateAsync(string token, bool isActive, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        var existing = await repository.GetAsync(token, ct);

        if (existing is null)
        {
            throw new ItemNotExistsException(ExceptionMessages.ItemNotFound);
        }

        var updated = existing with { IsActive = isActive };
        await repository.UpdateAsync(updated, ct);

        return updated;
    }

    public virtual async Task InvalidateAsync(string refreshToken, CancellationToken ct)
    {
        if (refreshToken is null)
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        var token = await repository.GetAsync(refreshToken, ct);

        if (token is null)
        {
            return;
        }

        var updatedToken = token with { IsActive = false };
        await repository.UpdateAsync(updatedToken, ct);
    }

    public virtual async Task<bool> ValidateAsync(Guid userId, string refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        var token = await repository.GetAsync(refreshToken, ct);

        return token != null && token.UserId == userId && token.IsActive && token.ExpirationDate > DateTime.UtcNow;
    }
}
