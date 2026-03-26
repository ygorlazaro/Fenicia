using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

/// <summary>
///     Handler responsible for creating a new product category.
///     Adds a new category to the database.
/// </summary>
public class AddProductCategoryHandler(DefaultContext db)
{
    /// <summary>
    ///     Creates a new product category.
    /// </summary>
    /// <param name="command">The command containing category details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created category with its details.</returns>
    public async Task<AddProductCategoryResponse> Handle(AddProductCategoryCommand command, CancellationToken ct)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name
        };

        db.BasicProductCategories.Add(category);

        await db.SaveChangesAsync(ct);

        return new AddProductCategoryResponse(category.Id, category.Name);
    }
}
