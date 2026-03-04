using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.GetById;

public class GetProductCategoryByIdHandler(DefaultContext context)
{
    public async Task<GetProductCategoryByIdResponse?> Handle(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await context.BasicProductCategories
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        if (category is null)
            return null;

        return new GetProductCategoryByIdResponse(category.Id, category.Name);
    }
}
