using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Queries;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

/// <summary>
///     Handler responsible for retrieving a specific product category by its ID.
/// </summary>
public class GetProductCategoryByIdHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves a product category by its ID.
    /// </summary>
    /// <param name="query">The query containing the category ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The category details if found, otherwise null.</returns>
    public async Task<GetProductCategoryByIdResponse?> Handle(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        return category switch
        {
            null => null,
            _ => new GetProductCategoryByIdResponse(category.Id, category.Name)
        };

    }
}