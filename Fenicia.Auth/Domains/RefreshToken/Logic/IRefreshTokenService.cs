namespace Fenicia.Auth.Domains.RefreshToken.Logic;

using Common;

/// <summary>
///     Provides functionality for managing refresh tokens in the authentication system.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    ///     Generates a new refresh token for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>An API response containing the generated refresh token string if successful.</returns>
    Task<ApiResponse<string>> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    ///     Validates a refresh token for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="refreshToken">The refresh token to validate.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>An API response containing a boolean indicating whether the token is valid.</returns>
    Task<ApiResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);

    /// <summary>
    ///     Invalidates a refresh token, preventing its future use.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>An API response indicating the result of the invalidation operation.</returns>
    Task<ApiResponse<object>> InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
