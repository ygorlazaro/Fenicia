using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class UpdateEmployeeHandler(DefaultContext db) : IRequestHandler<UpdateEmployeeCommand, UpdateEmployeeResponse?>
{

    public async Task<UpdateEmployeeResponse?> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await db.BasicEmployees
            .Include(employeeModel => employeeModel.Person)
            .Include(employeeModel => employeeModel.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
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

        if (command.Address != null)
        {
            var existingPersonAddress = employee.Person.PersonAddresses.FirstOrDefault();

            if (existingPersonAddress?.Address != null)
            {
                existingPersonAddress.Address.Street = command.Address.Street;
                existingPersonAddress.Address.Number = command.Address.Number;
                existingPersonAddress.Address.Complement = command.Address.Complement;
                existingPersonAddress.Address.Neighborhood = command.Address.Neighborhood;
                existingPersonAddress.Address.ZipCode = command.Address.ZipCode;
                existingPersonAddress.Address.StateId = command.Address.StateId;
                existingPersonAddress.Address.City = command.Address.City;
                existingPersonAddress.Address.Country = command.Address.Country;
            }
            else
            {
                var newAddress = new AddressModel
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
                db.AuthAddresses.Add(newAddress);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = employee.PersonId,
                    AddressId = newAddress.Id,
                };
                db.BasicPersonAddresses.Add(newPersonAddress);
            }
        }

        db.BasicEmployees.Update(employee);

        await db.SaveChangesAsync(ct);

        return new UpdateEmployeeResponse(employee.Id, employee.PositionId, employee.PersonId);
    }
}
