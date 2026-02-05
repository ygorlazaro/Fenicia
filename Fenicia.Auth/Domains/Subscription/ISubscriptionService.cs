using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Auth.Domains.Subscription;

public interface ISubscriptionService
{
    Task<SubscriptionResponse> CreateCreditsForOrderAsync(OrderModel order, List<OrderDetailModel> details, Guid companyId, CancellationToken cancellationToken);

    Task<List<Guid>> GetValidSubscriptionsAsync(Guid companyId, CancellationToken cancellationToken);
}
