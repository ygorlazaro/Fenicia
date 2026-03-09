using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllEmployeeForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllEmployeeForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicEmployees
            .AsNoTracking()
            .Include(e => e.Person)
            .OrderBy(e => e.Person.Name)
            .Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name))
            .ToListAsync(ct);
    }
}
