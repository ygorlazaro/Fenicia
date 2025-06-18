namespace Fenicia.Auth.Domains.Order;

public interface IOrderRepository
{
    Task<OrderModel> SaveOrderAsync(OrderModel order);
}
