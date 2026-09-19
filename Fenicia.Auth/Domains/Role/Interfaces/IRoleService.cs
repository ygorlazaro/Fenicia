using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role.Interfaces;

public interface IRoleService
{
    Task<RoleResponse?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default);

    Task<RoleResponse?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<List<RoleResponse>> GetRolesByIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default);
}