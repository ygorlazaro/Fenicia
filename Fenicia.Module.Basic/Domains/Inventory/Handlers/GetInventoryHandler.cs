using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.Handlers;

public class GetInventoryHandler(DefaultContext db)
{
    public async Task<InventoryResponse> Handle(GetInventoryQuery query, CancellationToken ct)
    {
        var products = db.BasicProducts
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage);

        var totalCostPrice = await db.BasicProducts.SumAsync(p => p.CostPrice ?? 0,
            ct);
        var totalSalesPrice = await db.BasicProducts.SumAsync(p => p.SalesPrice,
            ct);
        var totalQuantity = await db.BasicProducts.SumAsync(p => p.Quantity,
            ct);

        var inventoryDetailResponses = products.Select(p => new InventoryDetailResponse(p.Id,
            p.Name,
            p.Quantity,
            p.CostPrice,
            p.SalesPrice,
            p.CategoryId,
            p.Category.Name)).ToList();
        
        return new InventoryResponse
        {
            Items = inventoryDetailResponses,
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }
}
