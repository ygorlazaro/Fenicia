using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Auth;

public static class SubscriptionMapper
{
    public static SubscriptionResponse Map(SubscriptionModel subscription)
    {
        return new SubscriptionResponse
        {
            Id = subscription.Id,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            OrderId = subscription.OrderId
        };
    }
}
