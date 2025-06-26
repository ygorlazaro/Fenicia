using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Role.Data;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role.Logic;

public class RoleRepository(AuthContext authContext) : IRoleRepository
{
    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken)
    {
        return await authContext.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync(cancellationToken);
    }
}
