using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllEmployeeForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllEmployeeForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicEmployees
            .AsNoTracking()
            .Include(e => e.PersonModel)
            .OrderBy(e => e.PersonModel.Name)
            .Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.PersonModel.Name))
            .ToListAsync(ct);
    }
}

public record GetAllEmployeeForDataSourceResponse(Guid Id, string Name);
