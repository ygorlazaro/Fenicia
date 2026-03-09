using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetAll;

public class GetAllEmployeeHandler(DefaultContext context)
{
    public async Task<Pagination<List<GetAllEmployeeResponse>>> Handle(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var total = await context.BasicEmployees.CountAsync(ct);

        var employees = await context.BasicEmployees
            .Include(e => e.Person)
            .ThenInclude(p => p.State)
            .Include(e => e.Position)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = employees.Select(e => new GetAllEmployeeResponse(
            e.Id,
            e.PositionId,
            e.PersonId,
            e.Person.Name,
            e.Person.Email,
            e.Person.PhoneNumber,
            e.Person.Document,
            e.Person.Street,
            e.Person.Number,
            e.Person.Complement,
            e.Person.Neighborhood,
            e.Person.ZipCode,
            e.Person.StateId,
            e.Person.City,
            e.Position.Name,
            e.Person.State != null ? e.Person.State.Name : null)).ToList();

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }
}
