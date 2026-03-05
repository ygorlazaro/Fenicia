using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllProductForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllProductForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicProducts
            .OrderBy(p => p.Name)
            .Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name, p.SalesPrice))
            .ToListAsync(ct);
    }
}

public record GetAllProductForDataSourceResponse(Guid Id, string Name, decimal SalesPrice);
