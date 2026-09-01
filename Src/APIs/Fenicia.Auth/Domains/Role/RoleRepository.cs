using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleRepository(DefaultContext context) : Repository<RoleModel>(context), IRoleRepository
{
    public async Task<RoleModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
