using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.Handlers;

/// <summary>
///     Handler responsible for retrieving inventory data for products in a specific category.
/// </summary>
public class GetInventoryByCategoryHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves inventory data for products in a specific category.
    /// </summary>
    /// <param name="query">The query containing the category ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Inventory response for products in the category.</returns>
    public async Task<InventoryResponse> Handle(GetInventoryByCategoryQuery query, CancellationToken ct)
    {
        var products = db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).Include(p => p.Category).OrderBy(p => p.Quantity).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage);

        var totalCostPrice = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.CostPrice ?? 0, ct);
        var totalSalesPrice = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.SalesPrice, ct);
        var totalQuantity = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.Quantity, ct);

        return new InventoryResponse { Items = products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name)).ToList(), TotalCostPrice = totalCostPrice, TotalSalesPrice = totalSalesPrice, TotalQuantity = totalQuantity };
    }
}