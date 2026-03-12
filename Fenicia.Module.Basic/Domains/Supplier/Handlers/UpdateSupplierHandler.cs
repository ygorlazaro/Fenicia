using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

public class UpdateSupplierHandler(DefaultContext db)
{
    public async Task<UpdateSupplierResponse?> Handle(UpdateSupplierCommand command, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == command.Id,
                ct);

        if (supplier is null)
        {
            return null;
        }

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

        db.BasicSuppliers.Update(supplier);

        await db.SaveChangesAsync(ct);

        return new UpdateSupplierResponse(
            supplier.Id,
            supplier.Cnpj);
    }
}
