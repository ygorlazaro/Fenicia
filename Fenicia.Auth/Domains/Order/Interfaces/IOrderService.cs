using Fenicia.Common.DTOs.Auth.Order;

namespace Fenicia.Auth.Domains.Order.Interfaces;

public interface IOrderService
{
    Task<OrderResponse?> CreateAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default);
}