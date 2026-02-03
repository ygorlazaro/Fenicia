using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Order;

public class OrderRepository(AuthContext context) : IOrderRepository
{
    public async Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        return order;
    }
}
