namespace Fenicia.Auth.Domains.Subscription;

using Common;
using Common.Database.Responses;

using Fenicia.Common.Database.Models.Auth;

public interface ISubscriptionService
{
    Task<ApiResponse<SubscriptionResponse>> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken);

    Task<ApiResponse<List<Guid>>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken);
}
