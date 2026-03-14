using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

/// <summary>
/// Handler responsible for retrieving products by category ID.
/// Returns a paginated list of products belonging to a specific category.
/// </summary>
public class GetProductsByCategoryIdHandler(DefaultContext db)
{
    /// <summary>
    /// Retrieves products filtered by category.
    /// </summary>
    /// <param name="query">The query containing category ID, page number, and items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of products in the specified category.</returns>
    public async Task<List<GetProductsByCategoryIdResponse>> Handle(GetProductsByCategoryIdQuery query, CancellationToken ct)
    {
        return await db.BasicProducts
            .Where(p => p.CategoryId == query.CategoryId)
            .Select(p => new GetProductsByCategoryIdResponse(p.Id,
                p.Name,
                p.CostPrice,
                p.SalesPrice,
                p.Quantity,
                p.CategoryId,
                p.Category.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
