using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Handlers;

public class DeleteSupplierHandler(DefaultContext db) : IRequestHandler<DeleteSupplierCommand>
{

    public async Task Handle(DeleteSupplierCommand command, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null)
        {
            return;
        }

        supplier.Deleted = DateTime.Now;

        db.BasicSuppliers.Update(supplier);

        await db.SaveChangesAsync(ct);
    }
}