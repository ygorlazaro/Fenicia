using Fenicia.Common.DTOs.Auth.Order;

namespace Fenicia.Auth.Domains.Order.Interfaces;

/// <summary>
/// Interface for the order service, providing methods to manage orders.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates a new order based on the provided request.
    /// </summary>
    /// <param name="request">The request containing the order details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created order response or null if the operation failed.</returns>
    Task<OrderResponse?> CreateAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default);
}
