using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Subscription.Logic;

/// <summary>
/// Service interface for managing subscription-related operations
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Creates credits for a given order asynchronously
    /// </summary>
    /// <param name="order">The order model containing order information</param>
    /// <param name="details">List of order details associated with the order</param>
    /// <param name="companyId">The unique identifier of the company</param>
    /// <param name="cancellationToken">Token for canceling the operation if needed</param>
    /// <returns>ApiResponse containing the subscription response</returns>
    Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves valid subscriptions for a company asynchronously
    /// </summary>
    /// <param name="companyId">The unique identifier of the company</param>
    /// <param name="cancellationToken">Token for canceling the operation if needed</param>
    /// <returns>ApiResponse containing a list of subscription identifiers</returns>
    Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId,
        CancellationToken cancellationToken);
}
