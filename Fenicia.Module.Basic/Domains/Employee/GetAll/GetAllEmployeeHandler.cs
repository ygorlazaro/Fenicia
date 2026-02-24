using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetAll;

public class GetAllEmployeeHandler(BasicContext context)
{
    public async Task<List<GetAllEmployeeResponse>> Handle(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var employees = await context.Employees
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(e => new GetAllEmployeeResponse(e.Id, e.PositionId, e.PersonId))
            .ToListAsync(ct);

        return employees;
    }
}
