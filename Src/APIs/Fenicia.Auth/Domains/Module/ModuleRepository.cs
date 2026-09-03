using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleRepository(DefaultContext context) : Repository<ModuleModel>(context), IModuleRepository
{
    public Task<List<ModuleModel>> GetAllActiveAsync(
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        var query = from m in DbSet
            where m.Type != ModuleType.Auth && m.IsActive
            orderby m.SortOrder
            select m;

        return query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public Task<int> CountAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(m => m.Type != ModuleType.Auth && m.IsActive, cancellationToken);
    }

    public Task<List<ModuleModel>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(m => ids.Contains(m.Id)).OrderBy(m => m.Type).ToListAsync(cancellationToken);
    }

    public Task<ModuleModel?> GetByTypeAsync(ModuleType type, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(m => m.Type == type, cancellationToken);
    }
}