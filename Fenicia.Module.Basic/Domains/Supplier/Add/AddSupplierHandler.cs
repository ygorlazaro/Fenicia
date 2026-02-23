using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier.Add;

public class AddSupplierHandler(BasicContext context)
{
    public async Task<SupplierResponse> Handle(AddSupplierCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Cpf = command.Cpf,
            PhoneNumber = command.PhoneNumber ?? string.Empty,
            Street = command.Street ?? string.Empty,
            Number = command.Number ?? string.Empty,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode ?? string.Empty,
            StateId = command.StateId,
            City = command.City ?? string.Empty
        };

        var supplier = new SupplierModel
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id
        };

        context.Suppliers.Add(supplier);

        await context.SaveChangesAsync(ct);

        return new SupplierResponse(
            supplier.Id,
            supplier.Cnpj,
            new PersonResponse(
                person.Name,
                person.Email,
                person.Cpf,
                person.PhoneNumber,
                new AddressResponse(
                    person.City,
                    person.Complement,
                    person.Neighborhood,
                    person.Number,
                    person.StateId,
                    person.Street,
                    person.ZipCode)));
    }
}
