using Fenicia.Common.DTOs.Auth.RefreshToken;

namespace Fenicia.Auth.Domains.RefreshToken.Interfaces;

/// <summary>
/// Defines the contract for a service that manages refresh tokens, including methods for generating, retrieving, and validating refresh tokens.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generates a new refresh token for the specified user ID and returns it as a string.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to generate a refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a refresh token from the service based on the provided token string.
    /// </summary>
    /// <param name="token">The refresh token string to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<RefreshTokenResponse?> GetAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token for the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to validate the refresh token.</param>
    /// <param name="refreshToken">The refresh token string to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> ValidateAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
}
