using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Update;

public class UpdateProductCategoryHandler(DefaultContext context)
{
    public async Task<UpdateProductCategoryRecord?> Handle(UpdateProductCategoryCommand command, CancellationToken ct)
    {
        var category = await context.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;

        context.BasicProductCategories.Update(category);

        await context.SaveChangesAsync(ct);

        return new UpdateProductCategoryRecord(category.Id, category.Name);
    }
}
