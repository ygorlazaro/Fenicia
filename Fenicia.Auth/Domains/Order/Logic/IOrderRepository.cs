using Fenicia.Auth.Domains.Order.Data;

namespace Fenicia.Auth.Domains.Order.Logic;

public interface IOrderRepository
{
    Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken);
}
