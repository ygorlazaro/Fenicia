namespace Fenicia.Auth.Domains.Subscription.Logic;

using Common;

using Data;

using Order.Data;

using OrderDetail.Data;

public interface ISubscriptionService
{
    Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken);

    Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken);
}
