using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.UserRole.Interfaces;

/// <summary>
/// Service interface for managing user roles in the authentication domain.
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Gets companies associated with a user including their role.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role responses.</returns>
    Task<List<UserRoleResponse>> GetCompaniesByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets companies associated with a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user company responses.</returns>
    Task<List<UserCompanyResponse>> GetUserCompaniesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of user roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    Task<List<UserRoleModel>> GetUserRolesAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of user roles for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the count.</returns>
    Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user role by user ID and company ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user role model if found.</returns>
    Task<UserRoleModel?> GetUserRoleAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user is an admin for a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user is admin.</returns>
    Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has any role for a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user has any role.</returns>
    Task<bool> AnyIdAndCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role for a company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="role">The role name to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user has the role.</returns>
    Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a range of user roles.
    /// </summary>
    /// <param name="userRoles">The list of user roles to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertRangeAsync(List<UserRoleModel> userRoles, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a single user role.
    /// </summary>
    /// <param name="userRole">The user role to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the inserted user role.</returns>
    Task<UserRoleModel> InsertAsync(UserRoleModel userRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user role by ID.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles for a user by user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    Task<List<UserRoleModel>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all role models for a user by user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    Task<List<UserRoleModel>> GetUserRoleModelsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}