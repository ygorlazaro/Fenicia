using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class GetProductsByCategoryIdHandler(DefaultContext db)
{
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
