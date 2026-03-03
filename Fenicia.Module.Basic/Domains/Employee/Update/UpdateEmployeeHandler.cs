using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Update;

public class UpdateEmployeeHandler(DefaultContext context)
{
    public async Task<UpdateEmployeeResponse?> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await context.BasicEmployees
            .Include(e => e.PersonModel)
            .Include(e => e.PositionModel)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (employee is null)
        {
            return null;
        }

        employee.PositionId = command.PositionId;
        employee.PersonModel.Name = command.Name;
        employee.PersonModel.Email = command.Email;
        employee.PersonModel.Document = command.Document;
        employee.PersonModel.PhoneNumber = command.PhoneNumber;
        employee.PersonModel.Street = command.Street;
        employee.PersonModel.Number = command.Number;
        employee.PersonModel.Complement = command.Complement;
        employee.PersonModel.Neighborhood = command.Neighborhood;
        employee.PersonModel.ZipCode = command.ZipCode;
        employee.PersonModel.StateId = command.StateId;
        employee.PersonModel.City = command.City;

        context.BasicEmployees.Update(employee);

        await context.SaveChangesAsync(ct);

        return new UpdateEmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId);
    }
}
