using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleRepository(AuthContext context) : BaseRepository<RoleModel>(context), IRoleRepository
{
    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken)
    {
        return await context.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync(cancellationToken);
    }
}
