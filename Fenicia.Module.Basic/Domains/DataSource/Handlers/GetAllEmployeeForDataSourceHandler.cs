using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllEmployeeForDataSourceHandler(DefaultContext db)
{
    public async Task<List<GetAllEmployeeForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicEmployees
            .AsNoTracking()
            .OrderBy(e => e.Person.Name)
            .Select(e => new GetAllEmployeeForDataSourceResponse(e.Id,
                e.Person.Name))
            .ToListAsync(ct);
    }
}
