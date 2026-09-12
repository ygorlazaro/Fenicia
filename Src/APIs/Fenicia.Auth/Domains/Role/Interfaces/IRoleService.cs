using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role.Interfaces;

public interface IRoleService
{
    Task<GetAdminRoleResponse?> GetAdminAsync(CancellationToken cancellationToken = default);

    Task<RoleModel?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default);

    Task<RoleModel?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<List<RoleModel>> GetRolesByIdsAsync(List<Guid> roleIds, CancellationToken cancellationToken = default);
}