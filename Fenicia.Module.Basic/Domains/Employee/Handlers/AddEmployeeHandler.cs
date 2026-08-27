using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class AddEmployeeHandler(DefaultContext db) : IRequestHandler<AddEmployeeCommand, AddEmployeeResponse>
{

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
