using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.GetById;

public class GetEmployeeByIdHandler(DefaultContext context)
{
    public async Task<GetEmployeeByIdResponse?> Handle(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var employee = await context.BasicEmployees
            .Include(e => e.Person)
            .FirstOrDefaultAsync(e => e.Id == query.Id, ct);

        if (employee is null)
            return null;

        return new GetEmployeeByIdResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.Person.Name,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            employee.Person.Document,
            employee.Person.Street,
            employee.Person.Number,
            employee.Person.Complement,
            employee.Person.Neighborhood,
            employee.Person.ZipCode,
            employee.Person.StateId,
            employee.Person.City);
    }
}
