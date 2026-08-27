using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

public class UpdateSupplierHandler(DefaultContext db) : IRequestHandler<UpdateSupplierCommand, UpdateSupplierResponse?>
{

    public async Task<UpdateSupplierResponse?> Handle(UpdateSupplierCommand command, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
            .FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null)
        {
            return null;
        }

        supplier.Cnpj = command.Cnpj;
        supplier.Person.Name = command.Name;
        supplier.Person.Email = command.Email;
        supplier.Person.Document = command.Document;
        supplier.Person.PhoneNumber = command.PhoneNumber ?? string.Empty;

        if (command.Address != null)
        {
            var existingPersonAddress = supplier.Person.PersonAddresses.FirstOrDefault();

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
                    PersonId = supplier.PersonId,
                    AddressId = newAddress.Id
                };
                db.BasicPersonAddresses.Add(newPersonAddress);
            }
        }

        db.BasicSuppliers.Update(supplier);

        await db.SaveChangesAsync(ct);

        return new UpdateSupplierResponse(supplier.Id, supplier.Cnpj);
    }
}