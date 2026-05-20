using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

/// <summary>
///     Handler responsible for updating an existing product category.
/// </summary>
public class UpdateProductCategoryHandler(DefaultContext db) : IRequestHandler<UpdateProductCategoryCommand, UpdateProductCategoryResponse?>
{
    /// <summary>
    ///     Updates a product category.
    /// </summary>
    /// <param name="command">The command containing updated category details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated category details if found, otherwise null.</returns>
    public async Task<UpdateProductCategoryResponse?> Handle(UpdateProductCategoryCommand command, CancellationToken ct)
    {
        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;

        db.BasicProductCategories.Update(category);

        await db.SaveChangesAsync(ct);

        return new UpdateProductCategoryResponse(category.Id, category.Name);
    }
}