using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Order.Data;

namespace Fenicia.Auth.Domains.Order.Logic;

public class OrderRepository(AuthContext authContext) : IOrderRepository
{
    public async Task<OrderModel> SaveOrderAsync(OrderModel order,
        CancellationToken cancellationToken)
    {
        authContext.Orders.Add(order);

        await authContext.SaveChangesAsync(cancellationToken);

        return order;
    }
}
