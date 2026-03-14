using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

/// <summary>
/// Handler responsible for deleting a product category (soft delete).
/// </summary>
public class DeleteProductCategoryHandler(DefaultContext context)
{
    /// <summary>
    /// Deletes a product category by setting its Deleted timestamp.
    /// </summary>
    /// <param name="command">The command containing the category ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task Handle(DeleteProductCategoryCommand command, CancellationToken ct)
    {
        var category = await context.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id,
            ct);

        if (category is null)
        {
            return;
        }

        category.Deleted = DateTime.Now;

        context.BasicProductCategories.Update(category);

        await context.SaveChangesAsync(ct);
    }
}
