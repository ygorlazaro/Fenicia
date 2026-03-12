using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllProductCategoryForDataSourceHandler(DefaultContext db)
{
    public async Task<List<GetAllProductCategoryForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicProductCategories
            .OrderBy(pc => pc.Name)
            .Select(pc => new GetAllProductCategoryForDataSourceResponse(pc.Id,
                pc.Name))
            .ToListAsync(ct);
    }
}
