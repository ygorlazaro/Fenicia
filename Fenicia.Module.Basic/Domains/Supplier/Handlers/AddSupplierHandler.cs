using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

/// <summary>
///     Handler responsible for creating a new supplier.
///     Creates a new supplier along with their contact and address information.
/// </summary>
public class AddSupplierHandler(DefaultContext db)
{
    /// <summary>
    ///     Creates a new supplier.
    /// </summary>
    /// <param name="command">The command containing supplier details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created supplier with its details.</returns>
    public async Task<AddSupplierResponse> Handle(AddSupplierCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber
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

        var supplier = new SupplierModel
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id,
            Cnpj = command.Cnpj
        };

        if (address != null)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = address.Id
            };
            db.BasicPersonAddresses.Add(personAddress);
        }

        db.BasicSuppliers.Add(supplier);

        await db.SaveChangesAsync(ct);

        return new AddSupplierResponse(supplier.Id, supplier.Cnpj);
    }
}
