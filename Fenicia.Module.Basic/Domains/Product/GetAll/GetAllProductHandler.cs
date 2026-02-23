using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetAll;

public class GetAllProductHandler(BasicContext context)
{
    public async Task<List<ProductResponse>> Handle(GetAllProductQuery query, CancellationToken ct)
    {
        var products = await context.Products
            .Include(p => p.Category)
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
