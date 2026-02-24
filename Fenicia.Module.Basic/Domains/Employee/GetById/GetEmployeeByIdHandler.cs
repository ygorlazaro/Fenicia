using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetById;

public class GetEmployeeByIdHandler(BasicContext context)
{
    public async Task<GetEmployeeByIdResponse?> Handle(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        return await context.Employees
            .Select(e => new GetEmployeeByIdResponse(e.Id, e.PositionId, e.PersonId))
            .FirstOrDefaultAsync(e => e.Id == query.Id, ct);
    }
}