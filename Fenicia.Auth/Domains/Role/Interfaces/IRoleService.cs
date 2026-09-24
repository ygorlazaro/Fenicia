using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role.Interfaces;

/// <summary>
/// Represents a service for managing role entities in the authentication domain.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Retrieves a role entity by its name.
    /// </summary>
    /// <param name="roleName">The name of the role to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The role entity if found; otherwise, null.</returns>
    Task<RoleResponse?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a role entity by its ID.
    /// </summary>
    /// <param name="roleId">The ID of the role to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The role entity if found; otherwise, null.</returns>
    Task<RoleResponse?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of roles by their IDs.
    /// </summary>
    /// <param name="roleIds">The IDs of the roles to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of role entities.</returns>
    Task<List<RoleResponse>> GetRolesByIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default);
}
