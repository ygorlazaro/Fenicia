using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Update;

public class UpdateSupplierHandler(DefaultContext context)
{
    public async Task<UpdateSupplierResponse?> Handle(UpdateSupplierCommand command, CancellationToken ct)
    {
        var supplier = await context.BasicSuppliers
            .Include(s => s.PersonModel)
            .FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null)
        {
            return null;
        }

        supplier.Cnpj = command.Cnpj;
        supplier.PersonModel.Name = command.Name;
        supplier.PersonModel.Email = command.Email;
        supplier.PersonModel.Document = command.Document;
        supplier.PersonModel.PhoneNumber = command.PhoneNumber ?? string.Empty;
        supplier.PersonModel.Street = command.Street ?? string.Empty;
        supplier.PersonModel.Number = command.Number ?? string.Empty;
        supplier.PersonModel.Complement = command.Complement;
        supplier.PersonModel.Neighborhood = command.Neighborhood;
        supplier.PersonModel.ZipCode = command.ZipCode ?? string.Empty;
        supplier.PersonModel.StateId = command.StateId;
        supplier.PersonModel.City = command.City ?? string.Empty;

        context.BasicSuppliers.Update(supplier);

        await context.SaveChangesAsync(ct);

        return new UpdateSupplierResponse(
            supplier.Id,
            supplier.Cnpj);
    }
}
