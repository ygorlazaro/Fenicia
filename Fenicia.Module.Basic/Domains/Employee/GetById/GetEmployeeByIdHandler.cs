using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetById;

public class GetEmployeeByIdHandler(BasicContext context)
{
    public async Task<EmployeeResponse?> Handle(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var employee = await context.Employees
            .Include(e => e.Person)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == query.Id, ct);

        if (employee is null) return null;

        return new EmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.Position.Name,
            new PersonResponse(
                employee.Person.Name,
                employee.Person.Email,
                employee.Person.Document,
                employee.Person.PhoneNumber,
                new AddressResponse(
                    employee.Person.City,
                    employee.Person.Complement,
                    employee.Person.Neighborhood,
                    employee.Person.Number,
                    employee.Person.StateId,
                    employee.Person.Street,
                    employee.Person.ZipCode)));
    }
}
