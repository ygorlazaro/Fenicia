using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role.Interfaces;

public interface IRoleService
{
    Task<GetAdminRoleResponse?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default);

    Task<GetAdminRoleResponse?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<List<GetAdminRoleResponse>> GetRolesByIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default);
}