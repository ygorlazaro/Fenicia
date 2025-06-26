namespace Fenicia.Auth.Domains.RefreshToken.Logic;

using Data;

/// <summary>
///     Repository interface for managing refresh tokens in the authentication system.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    ///     Adds a new refresh token to the repository.
    /// </summary>
    /// <param name="refreshToken">The refresh token model to add.</param>
    void Add(RefreshTokenModel refreshToken);

    /// <summary>
    ///     Asynchronously saves changes to the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Validates a refresh token for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="refreshToken">The refresh token to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean indicating if the token is valid.</returns>
    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    ///     Invalidates a specific refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
