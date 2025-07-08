namespace Fenicia.Auth.Domains.Order.Logic;

using Data;

public interface IOrderRepository
{
    Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken = default);
}
