using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Commands;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class DeleteProductHandler(DefaultContext db) : IRequestHandler<DeleteProductCommand>
{

    public async Task Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product is null)
        {
            return;
        }

        product.Deleted = DateTime.Now;

        db.BasicProducts.Update(product);

        await db.SaveChangesAsync(ct);
    }
}