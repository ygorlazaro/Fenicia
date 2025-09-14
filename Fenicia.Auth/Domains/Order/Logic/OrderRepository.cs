namespace Fenicia.Auth.Domains.Order.Logic;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

public class OrderRepository : IOrderRepository
{
    private readonly AuthContext _authContext;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(AuthContext authContext, ILogger<OrderRepository> logger)
    {
        _authContext = authContext;
        _logger = logger;
    }

    public async Task<OrderModel> SaveOrderAsync(OrderModel order, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        try
        {
            _logger.LogInformation("Saving order {OrderId} to database", order.Id);

            _authContext.Orders.Add(order);
            await _authContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully saved order {OrderId}", order.Id);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save order {OrderId}", order.Id);
            throw new InvalidOperationException("Failed to save order to database", ex);
        }
    }
}
