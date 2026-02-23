using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.GetById;

public class GetProductCategoryByIdHandler(BasicContext context)
{
    public async Task<ProductCategoryResponse?> Handle(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await context.ProductCategories.FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        return category is null ? null : new ProductCategoryResponse(category.Id, category.Name);
    }
}
