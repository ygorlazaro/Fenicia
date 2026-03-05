using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllProductCategoryForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllProductCategoryForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicProductCategories
            .OrderBy(pc => pc.Name)
            .Select(pc => new GetAllProductCategoryForDataSourceResponse(pc.Id, pc.Name))
            .ToListAsync(ct);
    }
}

public record GetAllProductCategoryForDataSourceResponse(Guid Id, string Name);
