using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.GetById;

public class GetProductCategoryByIdHandler(BasicContext context)
{
    public async Task<GetProductCategoryByIdResponse?> Handle(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        return await context.ProductCategories
            .Select(pc => new GetProductCategoryByIdResponse(pc.Id, pc.Name))
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);
    }
}