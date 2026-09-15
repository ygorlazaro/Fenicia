using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role;

public class RoleService(IRoleRepository repository, RoleMapper roleMapper) : IRoleService
{
    public async Task<GetAdminRoleResponse?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByNameAsync(roleName, cancellationToken);

        return role is null ? null : roleMapper.MapToGetAdminRoleResponse(role);
    }

    public async Task<GetAdminRoleResponse?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByIdAsync(roleId, cancellationToken);

        return role is null ? null : roleMapper.MapToGetAdminRoleResponse(role);
    }

    public async Task<List<GetAdminRoleResponse>> GetRolesByIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetRolesByIdAsync(roleIds, cancellationToken);

        return [.. roles.Select(roleMapper.MapToGetAdminRoleResponse)];
    }
}