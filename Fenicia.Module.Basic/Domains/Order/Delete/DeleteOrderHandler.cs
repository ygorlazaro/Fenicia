using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Delete;

public class DeleteOrderHandler(DefaultContext context)
{
    public async Task Handle(DeleteOrderCommand command, CancellationToken ct)
    {
        var order = await context.BasicOrders
            .FirstOrDefaultAsync(o => o.Id == command.Id, ct);

        if (order is not null)
        {
            order.Deleted = DateTime.UtcNow;
            context.BasicOrders.Update(order);
            await context.SaveChangesAsync(ct);
        }
    }
}
