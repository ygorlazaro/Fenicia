using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.UserRole.Interfaces;

/// <summary>
/// Repository interface for managing user roles in the authentication domain.
/// </summary>
public interface IUserRoleRepository : IRepository<UserRoleModel>
{
    /// <summary>
    /// Gets all companies associated with a user including their roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    Task<List<UserRoleModel>> GetCompaniesByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all companies associated with a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken = default);

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
    /// Gets all roles for a user by user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    Task<List<UserRoleModel>> GetUserRolesByIdAsync(Guid userId, CancellationToken cancellationToken);
}