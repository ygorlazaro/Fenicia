using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Update;

public class UpdateSupplierHandler(BasicContext context)
{
    public async Task<SupplierResponse?> Handle(UpdateSupplierCommand command, CancellationToken ct)
    {
        var supplier = await context.Suppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null) return null;

        supplier.Cnpj = command.Cnpj;
        supplier.Person.Name = command.Name;
        supplier.Person.Email = command.Email;
        supplier.Person.Document = command.Document;
        supplier.Person.PhoneNumber = command.PhoneNumber ?? string.Empty;
        supplier.Person.Street = command.Street ?? string.Empty;
        supplier.Person.Number = command.Number ?? string.Empty;
        supplier.Person.Complement = command.Complement;
        supplier.Person.Neighborhood = command.Neighborhood;
        supplier.Person.ZipCode = command.ZipCode ?? string.Empty;
        supplier.Person.StateId = command.StateId;
        supplier.Person.City = command.City ?? string.Empty;

        context.Suppliers.Update(supplier);

        await context.SaveChangesAsync(ct);

        return new SupplierResponse(
            supplier.Id,
            supplier.Cnpj,
            new PersonResponse(
                supplier.Person.Name,
                supplier.Person.Email,
                supplier.Person.Document,
                supplier.Person.PhoneNumber,
                new AddressResponse(
                    supplier.Person.City,
                    supplier.Person.Complement,
                    supplier.Person.Neighborhood,
                    supplier.Person.Number,
                    supplier.Person.StateId,
                    supplier.Person.Street,
                    supplier.Person.ZipCode)));
    }
}
