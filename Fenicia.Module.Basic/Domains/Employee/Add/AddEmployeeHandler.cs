using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

namespace Fenicia.Module.Basic.Domains.Employee.Add;

public class AddEmployeeHandler(DefaultContext context)
{
    public async Task<AddEmployeeResponse> Handle(AddEmployeeCommand command, CancellationToken ct)
    {
        var person = new BasicPerson
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber,
            Street = command.Street,
            Number = command.Number,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode,
            StateId = command.StateId,
            City = command.City
        };

        var employee = new BasicEmployee
        {
            Id = command.Id,
            PositionId = command.PositionId,
            Person = person,
            PersonId = person.Id
        };

        context.BasicEmployees.Add(employee);

        await context.SaveChangesAsync(ct);

        return new AddEmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId);
    }
}
