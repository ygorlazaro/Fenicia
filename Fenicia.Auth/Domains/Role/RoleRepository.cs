using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleRepository(DefaultContext context) : Repository<RoleModel>(context)
{
    public async Task<RoleModel?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(r => r.Name == name && r.Deleted == null, ct);
    }
}
