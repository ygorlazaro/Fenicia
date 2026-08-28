using Fenicia.Auth.Domains.Role.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleService(DefaultContext db)
{
    public async Task<GetAdminRoleResponse?> GetAdminAsync(CancellationToken ct)
    {
        var role = await db.AuthRoles.FirstOrDefaultAsync(r => r.Name == "Admin", ct);

        return role switch
        {
            null => null,
            _ => new GetAdminRoleResponse(role.Id, role.Name)
        };
    }

    public async Task<RoleModel?> GetRoleAsync(string roleName, CancellationToken ct)
    {
        return await db.AuthRoles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
    }
}
