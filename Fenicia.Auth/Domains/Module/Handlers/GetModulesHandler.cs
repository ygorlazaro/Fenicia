using Fenicia.Auth.Domains.Module.Queries;
using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.Handlers;

/// <summary>
///     Handler responsible for retrieving available modules with pagination.
///     Returns a paginated list of modules excluding those with type Auth.
/// </summary>
/// <remarks>
///     This handler is used to provide a public catalog of available modules.
///     Modules are ordered by their Type enum value. The Auth module type is excluded
///     as it represents internal authentication functionality not available for subscription.
/// </remarks>
public class GetModulesHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves paginated modules, excluding Auth type modules.
    /// </summary>
    /// <param name="query">The query containing page number and items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing available modules.</returns>
    public async Task<Pagination<List<GetModuleResponse>>> Handle(GetModulesQuery query, CancellationToken ct)
    {
        var request = db.AuthModules.Where(m => m.Type != ModuleType.Auth && m.IsActive)
            .OrderBy(m => m.SortOrder)
            .Select(m => new GetModuleResponse(m.Id,
                m.Name,
                m.Type,
                m.Description,
                m.Icon,
                m.IsActive,
                m.SortOrder,
                m.Price));

        var modules = await request.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        var total = await request.CountAsync(ct);

        return new Pagination<List<GetModuleResponse>>(modules, total, query.Page, query.PerPage);
    }
}
