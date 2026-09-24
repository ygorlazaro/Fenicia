using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Role.Interfaces;

/// <summary>
/// Represents a repository for managing role entities in the authentication domain.
/// </summary>
public interface IRoleRepository : IRepository<RoleModel>
{
    /// <summary>
    /// Retrieves a role entity by its name.
    /// </summary>
    /// <param name="name">The name of the role to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The role entity if found; otherwise, null.</returns>
    Task<RoleModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of roles by their IDs.
    /// </summary>
    /// <param name="roleIds">The IDs of the roles to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of role entities.</returns>
    Task<List<RoleModel>> GetRolesByIdAsync(List<Guid> roleIds, CancellationToken cancellationToken = default);
}
