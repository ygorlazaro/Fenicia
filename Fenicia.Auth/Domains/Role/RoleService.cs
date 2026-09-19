using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role;

public class RoleService(RoleMapper mapper, IRoleRepository repository) : IRoleService
{
    public async Task<RoleResponse?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByNameAsync(roleName, cancellationToken);

        return role is null ? null : mapper.MapToRoleResponse(role);
    }

    public async Task<RoleResponse?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByIdAsync(roleId, cancellationToken);

        return role is null ? null : mapper.MapToRoleResponse(role);
    }

    public async Task<List<RoleResponse>> GetRolesByIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetRolesByIdAsync(roleIds, cancellationToken);

        return [.. roles.Select(mapper.MapToRoleResponse)];
    }
}