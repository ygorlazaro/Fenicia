using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

/// <summary>
/// Handler responsible for updating existing employee information.
/// Updates both the employee and associated person records.
/// </summary>
public class UpdateEmployeeHandler(DefaultContext db)
{
    /// <summary>
    /// Updates an existing employee's information.
    /// </summary>
    /// <param name="command">The update command containing the new employee data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated employee response if found, null if employee does not exist.</returns>
    public async Task<UpdateEmployeeResponse?> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await db.BasicEmployees
            .Include(employeeModel => employeeModel.Person)
            .FirstOrDefaultAsync(e => e.Id == command.Id,
                ct);

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

        db.BasicEmployees.Update(employee);

        await db.SaveChangesAsync(ct);

        return new UpdateEmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId);
    }
}
