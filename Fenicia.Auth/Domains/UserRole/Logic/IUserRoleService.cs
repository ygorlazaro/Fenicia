namespace Fenicia.Auth.Domains.UserRole.Logic;

using Common;

/// <summary>
///     Provides services for managing user roles and their relationships.
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    ///     Retrieves all roles associated with a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing an ApiResponse with an array of role names.
    ///     The operation returns all roles assigned to the specified user.
    /// </returns>
    Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    ///     Checks if a user has a specific role within a company asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="role">The role name to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing an ApiResponse with a boolean value.
    ///     Returns true if the user has the specified role in the company, false otherwise.
    /// </returns>
    Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken);
}
