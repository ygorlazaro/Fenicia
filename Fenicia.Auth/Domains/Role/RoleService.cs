using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role;

/// <summary>
/// Represents a service for managing role entities in the authentication domain.
/// </summary>
/// <param name="repository"></param>
public class RoleService(IRoleRepository repository) : IRoleService
{
    /// <summary>
    /// Retrieves a role entity by its name.
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<RoleResponse?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByNameAsync(roleName, cancellationToken);

        return role is null ? null : MapToRoleResponse(role);
    }

    /// <summary>
    /// Retrieves a role entity by its ID.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<RoleResponse?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByIdAsync(roleId, cancellationToken);

        return role is null ? null : MapToRoleResponse(role);
    }

    public async Task<List<RoleResponse>> GetRolesByIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetRolesByIdAsync(roleIds, cancellationToken);

        return [.. roles.Select(MapToRoleResponse)];
    }

    private static RoleResponse MapToRoleResponse(RoleModel role)
    {
        return new RoleResponse(role.Id, role.Name);
    }
}
