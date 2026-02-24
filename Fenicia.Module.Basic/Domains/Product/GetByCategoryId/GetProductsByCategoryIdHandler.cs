using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetByCategoryId;

public class GetProductsByCategoryIdHandler(BasicContext context)
{
    public async Task<List<GetProductsByCategoryIdResponse>> Handle(GetProductsByCategoryIdQuery query, CancellationToken ct)
    {
        return await context.Products
            .Where(p => p.CategoryId == query.CategoryId)
            .Select(p => new GetProductsByCategoryIdResponse(  p.Id,
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