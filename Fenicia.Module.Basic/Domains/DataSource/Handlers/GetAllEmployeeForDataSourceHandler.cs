using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllEmployeeForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllEmployeeForDataSourceQuery, List<GetAllEmployeeForDataSourceResponse>>
{

    public async Task<List<GetAllEmployeeForDataSourceResponse>> Handle(GetAllEmployeeForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicEmployees.AsNoTracking().OrderBy(e => e.Person.Name).Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name)).ToListAsync(ct);
    }
}
