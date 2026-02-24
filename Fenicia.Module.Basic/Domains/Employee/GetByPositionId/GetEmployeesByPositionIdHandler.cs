using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetByPositionId;

public class GetEmployeesByPositionIdHandler(BasicContext context)
{
    public async Task<List<GetEmployeesByPositionIdResponse>> Handle(GetEmployeesByPositionIdQuery query, CancellationToken ct)
    {
        return await context.Employees
            .Where(e => e.PositionId == query.PositionId)
            .Select(e => new GetEmployeesByPositionIdResponse(e.Id, e.PositionId, e.PersonId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

    }
}