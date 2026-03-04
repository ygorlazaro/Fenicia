using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllPositionForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllPositionForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicPositions
            .OrderBy(p => p.Name)
            .Select(p => new GetAllPositionForDataSourceResponse(p.Id, p.Name))
            .ToListAsync(ct);
    }
}

public record GetAllPositionForDataSourceResponse(Guid Id, string Name);
