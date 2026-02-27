using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

namespace Fenicia.Module.Basic.Domains.Supplier.Add;

public class AddSupplierHandler(DefaultContext context)
{
    public async Task<AddSupplierResponse> Handle(AddSupplierCommand command, CancellationToken ct)
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

        var supplier = new BasicSupplier
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id,
            Cnpj = command.Cnpj,
        };

        context.BasicSuppliers.Add(supplier);

        await context.SaveChangesAsync(ct);

        return new AddSupplierResponse(
            supplier.Id,
            supplier.Cnpj
        );
    }
}
