using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetAll;

public class GetAllProductHandler(DefaultContext context)
{
    public async Task<List<GetAllProductResponse>> Handle(GetAllProductQuery query, CancellationToken ct)
    {
        return await context.BasicProducts
            .Select(p => new GetAllProductResponse(p.Id,  p.Name, p.CostPrice, p.SalesPrice, p.Quantity, p.CategoryId, p.Category.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
