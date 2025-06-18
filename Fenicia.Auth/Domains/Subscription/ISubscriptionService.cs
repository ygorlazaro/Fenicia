using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.OrderDetail;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Subscription;

public interface ISubscriptionService
{
    Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(
        OrderModel order,
        List<OrderDetailModel> details,
        Guid companyId
    );
    Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId);
}
