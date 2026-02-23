using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.GetAll;

public class GetAllProductCategoryHandler(BasicContext context)
{
    public async Task<List<ProductCategoryResponse>> Handle(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        var categories = await context.ProductCategories.ToListAsync(ct);

        return categories.Select(c => new ProductCategoryResponse(c.Id, c.Name)).ToList();
    }
}
