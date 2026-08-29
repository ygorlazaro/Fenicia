using Fenicia.Auth.Domains.Role.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Role;

public class RoleService(RoleRepository repository)
{
    public async Task<GetAdminRoleResponse?> GetAdminAsync(CancellationToken ct)
    {
        var role = await repository.GetByNameAsync("Admin", ct);

        return role is null ? null : role.MapToGetAdminRoleResponse();
    }

    public async Task<RoleModel?> GetRoleAsync(string roleName, CancellationToken ct)
    {
        return await repository.GetByNameAsync(roleName, ct);
    }
}
