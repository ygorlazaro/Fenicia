using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Delete;

public class DeleteSupplierHandler(DefaultContext context)
{
    public async Task Handle(DeleteSupplierCommand command, CancellationToken ct)
    {
        var supplier = await context.BasicSuppliers.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null)
        {
            return;
        }

        supplier.Deleted = DateTime.Now;

        context.BasicSuppliers.Update(supplier);

        await context.SaveChangesAsync(ct);
    }
}
