using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Queries;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

public class GetProductCategoryByIdHandler(DefaultContext db)
{
    public async Task<GetProductCategoryByIdResponse?> Handle(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await db.BasicProductCategories
            .FirstOrDefaultAsync(c => c.Id == query.Id,
                ct);

        return category switch
        {
            null => null,
            _ => new GetProductCategoryByIdResponse(category.Id,
                category.Name)
        };

    }
}
