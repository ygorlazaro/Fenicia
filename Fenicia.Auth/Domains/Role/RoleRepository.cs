using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleRepository(DbContext context) : Repository<RoleModel>(context), IRoleRepository
{
    public Task<RoleModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public Task<List<RoleModel>> GetRolesByIdAsync(List<Guid> roleIds, CancellationToken cancellationToken)
    {
        var query = from r in DbSet
            where roleIds.Contains(r.Id)
            select r;

        return query.ToListAsync(cancellationToken);
    }
}