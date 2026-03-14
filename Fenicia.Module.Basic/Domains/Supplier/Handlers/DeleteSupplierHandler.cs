using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Supplier.Commands;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

/// <summary>
/// Handler responsible for deleting a supplier (soft delete).
/// </summary>
public class DeleteSupplierHandler(DefaultContext db)
{
    /// <summary>
    /// Deletes a supplier by setting its Deleted timestamp.
    /// </summary>
    /// <param name="command">The command containing the supplier ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task Handle(DeleteSupplierCommand command, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers.FirstOrDefaultAsync(s => s.Id == command.Id,
            ct);

        if (supplier is null)
        {
            return;
        }

        supplier.Deleted = DateTime.Now;

        db.BasicSuppliers.Update(supplier);

        await db.SaveChangesAsync(ct);
    }
}
