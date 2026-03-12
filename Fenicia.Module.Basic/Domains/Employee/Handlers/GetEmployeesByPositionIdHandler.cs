using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class GetEmployeesByPositionIdHandler(DefaultContext db)
{
    public async Task<List<GetEmployeesByPositionIdResponse>> Handle(GetEmployeesByPositionIdQuery query, CancellationToken ct)
    {
        return await db.BasicEmployees
            .Where(e => e.PositionId == query.PositionId)
            .Select(e => new GetEmployeesByPositionIdResponse(e.Id,
                e.PositionId,
                e.PersonId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

    }
}
