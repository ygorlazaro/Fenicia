using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.Logic;

/// <summary>
/// Repository for managing module-related database operations
/// </summary>
public class ModuleRepository(AuthContext authContext) : IModuleRepository
{
    /// <summary>
    /// Retrieves a paginated list of modules ordered by type
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="perPage">Number of items per page</param>
    /// <returns>List of module models</returns>
    public async Task<List<ModuleModel>> GetAllOrderedAsync(CancellationToken cancellationToken,
        int page = 1, int perPage = 10)
    {
        try
        {
            return await authContext
                .Modules.OrderBy(m => m.Type)
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves multiple modules by their IDs
    /// </summary>
    /// <param name="request">Collection of module IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching module models</returns>
    public async Task<List<ModuleModel>> GetManyOrdersAsync(IEnumerable<Guid> request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query =
                from module in authContext.Modules
                where request.Any(r => r == module.Id)
                orderby module.Type
                select module;

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Retrieves a module by its type
    /// </summary>
    /// <param name="moduleType">Type of the module to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Module model if found, null otherwise</returns>
    public async Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType,
        CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.Modules.FirstOrDefaultAsync(m => m.Type == moduleType, cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Counts the total number of modules
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count of modules</returns>
    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.Modules.CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
