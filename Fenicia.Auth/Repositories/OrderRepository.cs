using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;

namespace Fenicia.Auth.Repositories;

public class OrderRepository(AuthContext authContext) : IOrderRepository
{
    public async Task<OrderModel> SaveOrderAsync(OrderModel order)
    {
        authContext.Orders.Add(order);

        await authContext.SaveChangesAsync();

        return order;
    }
}