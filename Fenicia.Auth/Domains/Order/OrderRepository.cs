namespace Fenicia.Auth.Domains.Order;

using Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

public class OrderRepository : IOrderRepository
{
    private readonly AuthContext authContext;
    private readonly ILogger<OrderRepository> logger;

    public OrderRepository(AuthContext authContext, ILogger<OrderRepository> logger)
    {
        this.authContext = authContext;
        this.logger = logger;
    }

    public async Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        try
        {
            this.logger.LogInformation("Saving order {OrderID} to database", order.Id);

            this.authContext.Orders.Add(order);
            await this.authContext.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation("Successfully saved order {OrderID}", order.Id);
            return order;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to save order {OrderID}", order.Id);
            throw new InvalidOperationException("Failed to save order to database", ex);
        }
    }
}
