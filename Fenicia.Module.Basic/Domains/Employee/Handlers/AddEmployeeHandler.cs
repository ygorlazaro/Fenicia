using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Responses;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

/// <summary>
///     Handler responsible for creating new employees in the system.
///     Creates both the employee record and associated person record.
/// </summary>
public class AddEmployeeHandler(DefaultContext db)
{
    /// <summary>
    ///     Creates a new employee with the provided command data.
    /// </summary>
    /// <param name="command">The employee creation command containing all employee details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created employee response with employee, position, and person IDs.</returns>
    public async Task<AddEmployeeResponse> Handle(AddEmployeeCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber,
        };

        AddressModel? address = null;

        if (command.Address != null)
        {
            address = new AddressModel
            {
                Id = Guid.NewGuid(),
                Street = command.Address.Street,
                Number = command.Address.Number,
                Complement = command.Address.Complement,
                Neighborhood = command.Address.Neighborhood,
                ZipCode = command.Address.ZipCode,
                StateId = command.Address.StateId,
                City = command.Address.City,
                Country = command.Address.Country
            };
            db.AuthAddresses.Add(address);
        }

        var employee = new EmployeeModel
        {
            Id = command.Id,
            PositionId = command.PositionId,
            Person = person,
            PersonId = person.Id,
        };

        if (address != null)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = address.Id,
            };
            db.BasicPersonAddresses.Add(personAddress);
        }

        db.BasicEmployees.Add(employee);

        await db.SaveChangesAsync(ct);

        return new AddEmployeeResponse(employee.Id, employee.PositionId, employee.PersonId);
    }
}
