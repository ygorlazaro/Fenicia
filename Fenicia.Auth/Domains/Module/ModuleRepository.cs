using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module;

/// <summary>
/// Represents a repository for managing modules in the authentication domain.
/// </summary>
/// <param name="context"></param>
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

    /// <summary>
    /// Gets a module by its type.
    /// </summary>
    /// <param name="type">The type of the module to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<ModuleModel?> GetByTypeAsync(EnumModuleType type, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(m => m.Type == type, cancellationToken);
    }

    /// <summary>
    /// Gets a list of active modules that are not of the basic type. This method retrieves all modules that are currently active and have a type greater than the basic module type, ordered by their sort order.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<List<ModuleModel>> GetActiveModulesAsync(CancellationToken cancellationToken = default)
    {
        var query = CommonPublicQuery();

        return query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the total count of active modules that are not of the basic type. This method counts all modules that are currently active and have a type greater than the basic module type.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<int> GetTotalActiveModulesAsync(CancellationToken cancellationToken)
    {
        var query = CommonPublicQuery();

        return query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a queryable collection of active modules that are not of the basic type, ordered by their sort order. This method is used internally to build queries for retrieving active modules.
    /// </summary>
    /// <returns>A queryable collection of active modules.</returns>
    private IOrderedQueryable<ModuleModel> CommonPublicQuery()
    {
        return from m in DbSet
               where m.IsActive
                     && m.Type > EnumModuleType.Basic
               orderby m.SortOrder
               select m;
    }
}
