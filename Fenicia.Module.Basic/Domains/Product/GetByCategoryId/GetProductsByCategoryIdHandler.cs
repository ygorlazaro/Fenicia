using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetByCategoryId;

public class GetProductsByCategoryIdHandler(BasicContext context)
{
    public async Task<List<ProductResponse>> Handle(GetProductsByCategoryIdQuery query, CancellationToken ct)
    {
        var products = await context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == query.CategoryId)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return products.Select(p => new ProductResponse(
            p.Id,
            p.Name,
            p.CostPrice,
            p.SalesPrice,
            p.Quantity,
            p.CategoryId,
            p.Category.Name)).ToList();
    }
}
