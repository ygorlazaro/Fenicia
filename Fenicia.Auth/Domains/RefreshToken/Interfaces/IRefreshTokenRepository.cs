using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.RefreshToken.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages refresh token data, including methods for adding, retrieving, and updating refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Adds a new refresh token to the repository.
    /// </summary>
    /// <param name="token">The refresh token model to be added.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(RefreshTokenModel token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a refresh token from the repository based on the provided token string.
    /// </summary>
    /// <param name="token">The refresh token string to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<RefreshTokenModel?> GetAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing refresh token in the repository.
    /// </summary>
    /// <param name="token">The refresh token model to be updated.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(RefreshTokenModel token, CancellationToken cancellationToken = default);
}
