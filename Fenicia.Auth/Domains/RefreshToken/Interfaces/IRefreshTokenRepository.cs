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
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(RefreshTokenModel token);

    /// <summary>
    /// Retrieves a refresh token from the repository based on the provided token string.
    /// </summary>
    /// <param name="token">The refresh token string to retrieve.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<RefreshTokenModel?> GetAsync(string token);

    /// <summary>
    /// Updates an existing refresh token in the repository.
    /// </summary>
    /// <param name="token">The refresh token model to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(RefreshTokenModel token);
}
