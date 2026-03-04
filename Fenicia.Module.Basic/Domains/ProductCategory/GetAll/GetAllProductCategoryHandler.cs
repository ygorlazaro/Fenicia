using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.GetAll;

public class GetAllProductCategoryHandler(DefaultContext context)
{
    public async Task<Pagination<List<GetAllProductCategoryResponse>>> Handle(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        var total = await context.BasicProductCategories.CountAsync(ct);
        
        var categories = await context.BasicProductCategories
            .Select(pc => new GetAllProductCategoryResponse(pc.Id, pc.Name))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, query.Page, query.PerPage);
    }
}
