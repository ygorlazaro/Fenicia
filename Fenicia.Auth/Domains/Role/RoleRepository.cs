using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleRepository(AuthContext context) : BaseRepository<RoleModel>(context), IRoleRepository
{
    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken ct)
    {
        return await context.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync(ct);
    }

    public async Task<string?> GetByUserAndCompanyAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await context.UserRoles.Where(ur => ur.UserId == userId && ur.CompanyId == companyId).Select(ur => ur.Role.Name).FirstOrDefaultAsync(ct);
    }
}