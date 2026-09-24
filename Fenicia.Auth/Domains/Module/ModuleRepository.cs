using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

public class ModuleRepository(DbContext context) : Repository<ModuleModel>(context), IModuleRepository
{
    public Task<List<ModuleModel>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var query = from m in DbSet
                    where ids.Contains(m.Id)
                    orderby m.Type
                    select m;

        return query.ToListAsync(cancellationToken);
    }

    public Task<ModuleModel?> GetByTypeAsync(EnumModuleType type, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(m => m.Type == type, cancellationToken);
    }

    public Task<List<ModuleModel>> GetActiveModulesAsync(CancellationToken cancellationToken = default)
    {
        var query = CommonPublicQuery();

        return query.ToListAsync(cancellationToken);
    }

    public Task<int> GetTotalActiveModulesAsync(CancellationToken cancellationToken)
    {
        var query = CommonPublicQuery();

        return query.CountAsync(cancellationToken);
    }

    private IOrderedQueryable<ModuleModel> CommonPublicQuery()
    {
        return from m in DbSet
               where m.IsActive
                     && m.Type > EnumModuleType.Basic
               orderby m.SortOrder
               select m;
    }
}
