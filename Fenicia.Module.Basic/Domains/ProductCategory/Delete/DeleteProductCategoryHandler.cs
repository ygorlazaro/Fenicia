using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Delete;

public class DeleteProductCategoryHandler(DefaultContext context)
{
    public async Task Handle(DeleteProductCategoryCommand command, CancellationToken ct)
    {
        var category = await context.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (category is null)
        {
            return;
        }

        category.Deleted = DateTime.Now;

        context.BasicProductCategories.Update(category);

        await context.SaveChangesAsync(ct);
    }
}
