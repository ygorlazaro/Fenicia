using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Order.Data;

namespace Fenicia.Auth.Domains.Order.Logic;

/// <summary>
/// Repository for managing order-related database operations
/// </summary>
public class OrderRepository(AuthContext authContext, ILogger<OrderRepository> logger) : IOrderRepository
{
    /// <summary>
    /// Saves an order to the database asynchronously
    /// </summary>
    /// <param name="order">The order model to save</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>The saved order model with updated information</returns>
    /// <exception cref="ArgumentNullException">Thrown when order is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when database operation fails</exception>
    public async Task<OrderModel> SaveOrderAsync(OrderModel order,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        try
        {
            logger.LogInformation("Saving order {OrderId} to database", order.Id);

            authContext.Orders.Add(order);
            await authContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully saved order {OrderId}", order.Id);
            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save order {OrderId}", order.Id);
            throw new InvalidOperationException("Failed to save order to database", ex);
        }
    }
}
