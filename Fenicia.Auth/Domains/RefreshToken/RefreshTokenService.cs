using System.Security.Cryptography;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Common.DTOs.Auth.RefreshToken;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.RefreshToken;

/// <summary>
/// Represents a service for managing refresh tokens, including generating new tokens, retrieving existing tokens, and validating tokens for specific users.
/// </summary>
/// <param name="repository"></param>
public sealed class RefreshTokenService(IRefreshTokenRepository repository) : IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token for the specified user ID and returns it as a string. The generated token is stored in the repository with an expiration date of 7 days from the current UTC time.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to generate a refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated refresh token string.</returns>
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

    /// <summary>
    /// Retrieves a refresh token from the service based on the provided token string. If the token is found, it is mapped to a RefreshTokenResponse object; if not found, null is returned.
    /// </summary>
    /// <param name="token">The refresh token string to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refresh token response if found; otherwise, null.</returns>
    public async Task<RefreshTokenResponse?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var tokenModel = await repository.GetAsync(token);

        return tokenModel is null ? null : RefreshTokenMapper.MapToRefreshTokenResponse(tokenModel);
    }

    /// <summary>
    /// Validates a refresh token for the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to validate the refresh token.</param>
    /// <param name="refreshToken">The refresh token string to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<bool> ValidateAsync(
        Guid userId,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new BadRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        var token = await repository.GetAsync(refreshToken);

        return token != null && token.UserId == userId && token.IsActive && token.ExpirationDate > DateTime.UtcNow;
    }
}
