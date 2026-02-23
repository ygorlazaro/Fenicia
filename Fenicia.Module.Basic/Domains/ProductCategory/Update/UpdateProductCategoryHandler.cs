using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Update;

public class UpdateProductCategoryHandler(BasicContext context)
{
    public async Task<ProductCategoryResponse?> Handle(UpdateProductCategoryCommand command, CancellationToken ct)
    {
        var category = await context.ProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (category is null) return null;

        category.Name = command.Name;

        context.ProductCategories.Update(category);

        await context.SaveChangesAsync(ct);

        return new ProductCategoryResponse(category.Id, category.Name);
    }
}
