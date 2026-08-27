using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Order.Commands;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

public class DeleteOrderHandler(DefaultContext db) : IRequestHandler<DeleteOrderCommand>
{

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
