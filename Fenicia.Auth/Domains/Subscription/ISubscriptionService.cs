using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Subscription;

public interface ISubscriptionService
{
    Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken);

    Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken);
}
