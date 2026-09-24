using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.UserRole;

/// <summary>
/// Service implementation for managing user roles in the authentication domain.
/// </summary>
/// <param name="repository">The user role repository.</param>
public class UserRoleService(IUserRoleRepository repository) : IUserRoleService
{
    /// <summary>
    /// Gets companies associated with a user including their role.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role responses.</returns>
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRoles = await repository.GetCompaniesByUserAsync(userId, cancellationToken);

        return [.. userRoles.Select(UserRoleMapper.MapToUserRoleResponse)];
    }

    /// <summary>
    /// Gets companies associated with a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user company responses.</returns>
    public async Task<List<UserCompanyResponse>> GetUserCompaniesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRoles = await repository.GetUserCompaniesAsync(userId, cancellationToken);

        return [.. userRoles.Select(UserRoleMapper.MapToUserCompanyResponse)];
    }

    /// <summary>
    /// Gets a paginated list of user roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role responses.</returns>
    public async Task<List<UserRoleResponse>> GetUserRolesAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        var userRoles = await repository.GetUserRolesAsync(userId, page, perPage, cancellationToken);

        return [.. userRoles.Select(UserRoleMapper.MapToUserRoleResponse)];
    }

    /// <summary>
    /// Gets the total count of user roles for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the count.</returns>
    public Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return repository.CountUserRolesAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Gets a user role by user ID and company ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user role response if found.</returns>
    public async Task<UserRoleResponse?> GetUserRoleAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var userRole = await repository.GetUserRoleAsync(userId, companyId, cancellationToken);

        return userRole is null ? null : UserRoleMapper.MapToUserRoleResponse(userRole);
    }

    /// <summary>
    /// Checks if a user is an admin for a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user is admin.</returns>
    public Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return repository.IsAdminAsync(userId, companyId, cancellationToken);
    }

    /// <summary>
    /// Checks if a user has any role for a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user has any role.</returns>
    public Task<bool> AnyIdAndCompanyAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return repository.AnyIdAndCompanyAsync(userId, companyId, cancellationToken);
    }

    /// <summary>
    /// Checks if a user has a specific role for a company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="role">The role name to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user has the role.</returns>
    public Task<bool> HasRoleAsync(
        Guid userId,
        Guid companyId,
        string role,
        CancellationToken cancellationToken = default)
    {
        return repository.HasRoleAsync(userId, companyId, role, cancellationToken);
    }

    /// <summary>
    /// Inserts a range of user roles.
    /// </summary>
    /// <param name="userRoles">The list of user roles to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task InsertRangeAsync(List<UserRoleModel> userRoles, CancellationToken cancellationToken = default)
    {
        return repository.InsertRangeAsync(userRoles, cancellationToken);
    }

    /// <summary>
    /// Inserts a single user role.
    /// </summary>
    /// <param name="userRole">The user role to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the inserted user role model.</returns>
    public Task<UserRoleModel> InsertAsync(UserRoleModel userRole, CancellationToken cancellationToken = default)
    {
        return repository.InsertAsync(userRole, cancellationToken);
    }

    /// <summary>
    /// Deletes a user role by ID.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(roleId, cancellationToken);
    }

    /// <summary>
    /// Gets all roles for a user by user ID (returns models for internal use).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    public Task<List<UserRoleModel>> GetUserRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetUserRolesByIdAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Gets all role models for a user by user ID (for internal use).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    public Task<List<UserRoleModel>> GetUserRoleModelsByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetCompaniesByUserAsync(userId, cancellationToken);
    }
}
