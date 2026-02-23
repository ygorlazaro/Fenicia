using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Update;

public class UpdateEmployeeHandler(BasicContext context)
{
    public async Task<EmployeeResponse?> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await context.Employees
            .Include(e => e.Person)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (employee is null) return null;

        employee.PositionId = command.PositionId;
        employee.Person.Name = command.Name;
        employee.Person.Email = command.Email;
        employee.Person.Cpf = command.Cpf;
        employee.Person.PhoneNumber = command.PhoneNumber ?? string.Empty;
        employee.Person.Street = command.Street ?? string.Empty;
        employee.Person.Number = command.Number ?? string.Empty;
        employee.Person.Complement = command.Complement;
        employee.Person.Neighborhood = command.Neighborhood;
        employee.Person.ZipCode = command.ZipCode ?? string.Empty;
        employee.Person.StateId = command.StateId;
        employee.Person.City = command.City ?? string.Empty;

        context.Employees.Update(employee);

        await context.SaveChangesAsync(ct);

        return new EmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.Position.Name,
            new PersonResponse(
                employee.Person.Name,
                employee.Person.Email,
                employee.Person.Cpf,
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
