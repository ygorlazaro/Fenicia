using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleRepository(DefaultContext context) : Repository<ModuleModel>(context)
{
    public async Task<List<ModuleModel>> GetAllActiveAsync(int page, int perPage, CancellationToken cancellationToken)
    {
        var query = from m in DbSet
                    where m.Type != ModuleType.Auth && m.IsActive
                    orderby m.SortOrder
                    select m;

        return await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAllActiveAsync(CancellationToken cancellationToken)
    {
        return await DbSet.CountAsync(m => m.Type != ModuleType.Auth && m.IsActive, cancellationToken);
    }

    public async Task<List<ModuleModel>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await DbSet.Where(m => ids.Contains(m.Id)).OrderBy(m => m.Type).ToListAsync(cancellationToken);
    }

    public async Task<ModuleModel?> GetByTypeAsync(ModuleType type, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(m => m.Type == type, cancellationToken);
    }
}
