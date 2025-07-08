namespace Fenicia.Auth.Domains.Order.Logic;

using Contexts;

using Data;

public class OrderRepository(AuthContext authContext, ILogger<OrderRepository> logger) : IOrderRepository
{
    public async Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        try
        {
            logger.LogInformation(message: "Saving order {OrderId} to database", order.Id);

            authContext.Orders.Add(order);
            await authContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(message: "Successfully saved order {OrderId}", order.Id);
            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Failed to save order {OrderId}", order.Id);
            throw new InvalidOperationException(message: "Failed to save order to database", ex);
        }
    }
}
