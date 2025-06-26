using Fenicia.Auth.Domains.Order.Data;

namespace Fenicia.Auth.Domains.Order.Logic;

/// <summary>
/// Represents a repository for managing order-related operations in the system.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Asynchronously saves an order to the repository.
    /// </summary>
    /// <param name="order">The order model to be saved. Must not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the save operation.</param>
    /// <returns>A task containing the saved order model with updated information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the order parameter is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the save operation fails.</exception>
    Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken = default);
}
