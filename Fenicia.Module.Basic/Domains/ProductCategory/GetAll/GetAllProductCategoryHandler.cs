using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.GetAll;

public class GetAllProductCategoryHandler(BasicContext context)
{
    public async Task<List<GetAllProductCategoryResponse>> Handle(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        return await context.ProductCategories
            .Select(pc => new GetAllProductCategoryResponse(pc.Id, pc.Name))
            .ToListAsync(ct);
    }
}