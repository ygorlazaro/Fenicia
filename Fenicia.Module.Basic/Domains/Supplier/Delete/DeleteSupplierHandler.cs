using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Delete;

public class DeleteSupplierHandler(BasicContext context)
{
    public async Task Handle(DeleteSupplierCommand command, CancellationToken ct)
    {
        var supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null)
        {
            return;
        }

        supplier.Deleted = DateTime.Now;

        context.Suppliers.Update(supplier);

        await context.SaveChangesAsync(ct);
    }
}
