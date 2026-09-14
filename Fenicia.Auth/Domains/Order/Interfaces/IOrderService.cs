using Fenicia.Common.DTOs.Auth.Order;

namespace Fenicia.Auth.Domains.Order.Interfaces;

public interface IOrderService
{
    Task<CreateNewOrderResponse?> CreateAsync(
        CreateNewOrderCommand command,
        CancellationToken cancellationToken = default);
}