using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Auth.Domains.User.Interfaces;

/// <summary>
/// Service interface for managing users in the authentication domain.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a paginated list of all users based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with paginated user responses.</returns>
    Task<Pagination<List<UserResponse>>> GetAllAsync(
        UserRequest query,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user response if found.</returns>
    Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user model by email address, or default if not found (for authentication).
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user model if found.</returns>
    Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user response by email address, or default if not found.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user response if found.</returns>
    Task<UserResponse?> FirstByEmailOrDefaultAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures the logged-in user can access the requested user's data.
    /// </summary>
    /// <param name="loggedInUserId">The ID of the logged-in user.</param>
    /// <param name="requestedUserId">The ID of the requested user.</param>
    /// <param name="companyId">The optional company ID for access validation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EnsureCanAccessUserAsync(
        Guid loggedInUserId,
        Guid requestedUserId,
        Guid? companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The user request data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the created user response.</returns>
    Task<UserResponse> CreateAsync(UserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="request">The user request data with updated information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the updated user response.</returns>
    Task<UserResponse> UpdateAsync(UserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user (soft delete).
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user's password.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="plainPassword">The new plain password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the updated user response.</returns>
    Task<UserResponse> UpdatePasswordAsync(
        Guid userId,
        string plainPassword,
        CancellationToken cancellationToken = default);
}
