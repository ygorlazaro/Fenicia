using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Commands;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

/// <summary>
///     Handler responsible for deleting a product (soft delete).
///     Sets the Deleted timestamp to mark the product as removed.
/// </summary>
public class DeleteProductHandler(DefaultContext db) : IRequestHandler<DeleteProductCommand>
{
    /// <summary>
    ///     Deletes a product by setting its Deleted timestamp.
    /// </summary>
    /// <param name="command">The command containing the product ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
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