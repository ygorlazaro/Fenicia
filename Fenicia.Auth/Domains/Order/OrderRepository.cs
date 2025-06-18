using Fenicia.Auth.Contexts;

namespace Fenicia.Auth.Domains.Order;

public class OrderRepository(AuthContext authContext) : IOrderRepository
{
    public async Task<OrderModel> SaveOrderAsync(OrderModel order)
    {
        authContext.Orders.Add(order);

        await authContext.SaveChangesAsync();

        return order;
    }
}
