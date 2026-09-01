using Fenicia.Auth.Domains.Role.DTOs;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleService(IRoleRepository repository) : IRoleService
{
    public async Task<GetAdminRoleResponse?> GetAdminAsync(CancellationToken cancellationToken = default)
    {
        var role = await repository.GetByNameAsync("Admin", cancellationToken);

        return role?.MapToGetAdminRoleResponse();
    }

    public async Task<RoleModel?> GetRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await repository.GetByNameAsync(roleName, cancellationToken);
    }

    public async Task<RoleModel?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(roleId, cancellationToken);
    }

    public async Task<List<RoleModel>> GetRolesByIdsAsync(List<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        return await repository.Query()
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}
