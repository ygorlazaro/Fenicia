using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Order;

public interface IOrderRepository
{
    Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken = default);
}
