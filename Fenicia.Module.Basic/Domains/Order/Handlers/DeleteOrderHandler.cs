using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Order.Commands;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

/// <summary>
///     Handler responsible for soft-deleting orders.
/// </summary>
public class DeleteOrderHandler(DefaultContext db) : IRequestHandler<DeleteOrderCommand>
{
    /// <summary>
    ///     Soft-deletes an order by setting the Deleted timestamp.
    /// </summary>
    /// <param name="command">Command containing the order ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task Handle(DeleteOrderCommand command, CancellationToken ct)
    {
        var order = await db.BasicOrders.FirstOrDefaultAsync(o => o.Id == command.Id, ct);

        if (order is not null)
        {
            order.Deleted = DateTime.UtcNow;
            db.BasicOrders.Update(order);
            await db.SaveChangesAsync(ct);
        }
    }
}
