namespace Fenicia.Auth.Domains.User.Logic;

using Common;

using Data;

using Token.Data;

/// <summary>
///     Provides user management operations including authentication, creation, and profile management.
/// </summary>
public interface IUserService
{
    /// <summary>
    ///     Authenticates a user and returns their profile information for login.
    /// </summary>
    /// <param name="request">The token request containing login credentials.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>API response containing the user profile information.</returns>
    /// <remarks>
    ///     Implementation should include:
    ///     - Validation of the request
    ///     - Error logging for invalid credentials
    ///     - Exception handling with appropriate logging
    /// </remarks>
    Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Creates a new user in the system.
    /// </summary>
    /// <param name="request">The user creation request containing user details.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>API response containing the created user profile.</returns>
    /// <remarks>
    ///     Implementation should include:
    ///     - Validation of user data
    ///     - Duplicate check logging
    ///     - Exception handling with appropriate logging
    /// </remarks>
    Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Checks if a user exists in a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>API response indicating whether the user exists in the company.</returns>
    Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves user information for token refresh operations.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>API response containing the user profile information.</returns>
    /// <remarks>
    ///     Implementation should include:
    ///     - User existence validation
    ///     - Error logging for invalid user ID
    ///     - Exception handling with appropriate logging
    /// </remarks>
    Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves user information based on their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>API response containing the user profile information.</returns>
    /// <remarks>
    ///     Implementation should include:
    ///     - Email format validation
    ///     - Error logging for non-existent email
    ///     - Exception handling with appropriate logging
    /// </remarks>
    Task<ApiResponse<UserResponse>> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    ///     Changes the password for a specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="password">The new password to set.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>API response containing the updated user profile.</returns>
    /// <remarks>
    ///     Implementation should include:
    ///     - Password complexity validation
    ///     - User existence validation
    ///     - Error logging for validation failures
    ///     - Exception handling with appropriate logging
    /// </remarks>
    Task<ApiResponse<UserResponse>> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
}
