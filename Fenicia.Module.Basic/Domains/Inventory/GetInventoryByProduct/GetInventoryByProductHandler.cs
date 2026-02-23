using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryByProduct;

public class GetInventoryByProductHandler(BasicContext context)
{
    public async Task<InventoryResponse> Handle(GetInventoryByProductQuery query, CancellationToken ct)
    {
        var products = await context.Products
            .Where(p => p.Id == query.ProductId)
            .Include(p => p.Category)
            .OrderBy(p => p.Quantity)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var totalCostPrice = await context.Products.Where(p => p.Id == query.ProductId).SumAsync(p => p.CostPrice ?? 0, ct);
        var totalSalesPrice = await context.Products.Where(p => p.Id == query.ProductId).SumAsync(p => p.SalesPrice, ct);
        var totalQuantity = await context.Products.Where(p => p.Id == query.ProductId).SumAsync(p => p.Quantity, ct);

        return new InventoryResponse
        {
            Items = products.Select(p => new InventoryDetailResponse(p.Id, p.Name, p.Quantity, p.CostPrice, p.SalesPrice, p.CategoryId, p.Category?.Name ?? "")).ToList(),
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }
}
