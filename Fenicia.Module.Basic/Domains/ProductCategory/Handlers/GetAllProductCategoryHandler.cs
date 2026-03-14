using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Queries;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

/// <summary>
/// Handler responsible for retrieving all product categories with pagination.
/// Returns a paginated list of categories.
/// </summary>
public class GetAllProductCategoryHandler(DefaultContext db)
{
    /// <summary>
    /// Retrieves paginated product categories.
    /// </summary>
    /// <param name="query">The query containing page number and items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing categories.</returns>
    public async Task<Pagination<List<GetAllProductCategoryResponse>>> Handle(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        var total = await db.BasicProductCategories.CountAsync(ct);
    
        var categories = await db.BasicProductCategories
            .Select(pc => new GetAllProductCategoryResponse(pc.Id,
                pc.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories,
            total,
            query.Page,
            query.PerPage);
    }
}
