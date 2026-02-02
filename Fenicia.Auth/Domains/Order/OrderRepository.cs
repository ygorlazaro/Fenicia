using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Order;

public class OrderRepository(AuthContext context, ILogger<OrderRepository> logger) : IOrderRepository
{
    public async Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        try
        {
            logger.LogInformation("Saving order {OrderID} to database", order.Id);

            context.Orders.Add(order);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully saved order {OrderID}", order.Id);

            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save order {OrderID}", order.Id);

            throw new InvalidOperationException("Failed to save order to database", ex);
        }
    }
}
