using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Inventory.Handlers;

public class GetInventoryByCategoryHandler(DefaultContext db) : IRequestHandler<GetInventoryByCategoryQuery, InventoryResponse>
{

    public async Task<InventoryResponse> Handle(GetInventoryByCategoryQuery query, CancellationToken ct)
    {
        var products = db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).Include(p => p.Category).OrderBy(p => p.Quantity).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage);

        var totalCostPrice = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.CostPrice ?? 0, ct);
        var totalSalesPrice = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.SalesPrice, ct);
        var totalQuantity = await db.BasicProducts.Where(p => p.CategoryId == query.CategoryId).SumAsync(p => p.Quantity, ct);

        return new InventoryResponse
        {
            Items = products.Select(p => new InventoryDetailResponse(p.Id,
                    p.Name,
                    p.Quantity,
                    p.CostPrice,
                    p.SalesPrice,
                    p.CategoryId,
                    p.Category.Name))
                .ToList(),
            TotalCostPrice = totalCostPrice,
            TotalSalesPrice = totalSalesPrice,
            TotalQuantity = totalQuantity
        };
    }
}
