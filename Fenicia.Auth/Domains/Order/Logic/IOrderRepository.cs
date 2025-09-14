namespace Fenicia.Auth.Domains.Order.Logic;

using Fenicia.Common.Database.Models.Auth;

public interface IOrderRepository
{
    Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken = default);
}
