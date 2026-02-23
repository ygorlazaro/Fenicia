using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetAll;

public class GetAllEmployeeHandler(BasicContext context)
{
    public async Task<List<EmployeeResponse>> Handle(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var employees = await context.Employees
            .Include(e => e.Person)
            .Include(e => e.Position)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return employees.Select(e => new EmployeeResponse(
            e.Id,
            e.PositionId,
            e.Position.Name,
            new PersonResponse(
                e.Person.Name,
                e.Person.Email,
                e.Person.Document,
                e.Person.PhoneNumber,
                new AddressResponse(
                    e.Person.City,
                    e.Person.Complement,
                    e.Person.Neighborhood,
                    e.Person.Number,
                    e.Person.StateId,
                    e.Person.Street,
                    e.Person.ZipCode)))).ToList();
    }
}
