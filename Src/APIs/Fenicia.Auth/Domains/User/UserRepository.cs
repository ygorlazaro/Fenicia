using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserRepository(DefaultContext context) : Repository<UserModel>(context)
{
    public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email && u.Deleted == null, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email && u.Deleted == null, ct);
    }

    public async Task<List<UserRoleModel>> GetCompaniesAsync(Guid userId, CancellationToken ct = default)
    {
        return await Context.AuthUserRoles
            .Where(ur => ur.UserId == userId && ur.Deleted == null)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .ToListAsync(ct);
    }
}
