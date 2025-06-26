using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Subscription.Logic;

public interface ISubscriptionService
{
    Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId, CancellationToken cancellationToken);
    Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId,
        CancellationToken cancellationToken);
}
