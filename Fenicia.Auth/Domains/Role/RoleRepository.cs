using Fenicia.Auth.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleRepository(AuthContext authContext) : IRoleRepository
{
    public async Task<RoleModel?> GetAdminRoleAsync()
    {
        return await authContext.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync();
    }
}
