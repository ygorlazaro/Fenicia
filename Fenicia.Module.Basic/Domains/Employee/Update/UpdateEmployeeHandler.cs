using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Update;

public class UpdateEmployeeHandler(DefaultContext context)
{
    public async Task<UpdateEmployeeResponse?> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await context.BasicEmployees
            .Include(e => e.Person)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (employee is null)
        {
            return null;
        }

        employee.PositionId = command.PositionId;
        employee.Person.Name = command.Name;
        employee.Person.Email = command.Email;
        employee.Person.Document = command.Document;
        employee.Person.PhoneNumber = command.PhoneNumber;
        employee.Person.Street = command.Street;
        employee.Person.Number = command.Number;
        employee.Person.Complement = command.Complement;
        employee.Person.Neighborhood = command.Neighborhood;
        employee.Person.ZipCode = command.ZipCode;
        employee.Person.StateId = command.StateId;
        employee.Person.City = command.City;

        context.BasicEmployees.Update(employee);

        await context.SaveChangesAsync(ct);

        return new UpdateEmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId);
    }
}
