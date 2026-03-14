using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.Handlers;

/// <summary>
///     Handler responsible for retrieving inventory data for a specific product.
/// </summary>
public class GetInventoryByProductHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves inventory data for a specific product.
    /// </summary>
    /// <param name="query">The query containing the product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Inventory response for the specified product.</returns>
    public async Task<InventoryResponse> Handle(GetInventoryByProductQuery query, CancellationToken ct)
    {
        var products = db.BasicProducts.Where(p => p.Id == query.ProductId).Include(p => p.Category).OrderBy(p => p.Quantity).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage);

        var totalCostPrice = await db.BasicProducts.Where(p => p.Id == query.ProductId).SumAsync(p => p.CostPrice ?? 0, ct);
        var totalSalesPrice = await db.BasicProducts.Where(p => p.Id == query.ProductId).SumAsync(p => p.SalesPrice, ct);
        var totalQuantity = await db.BasicProducts.Where(p => p.Id == query.ProductId).SumAsync(p => p.Quantity, ct);

        return new InventoryResponse { Items = products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category.Name)).ToList(), TotalCostPrice = totalCostPrice, TotalSalesPrice = totalSalesPrice, TotalQuantity = totalQuantity };
    }
}