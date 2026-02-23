using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;

public record CreateCreditsForOrderResponse(
    Guid Id,
    Guid CompanyId,
    DateTime StartDate,
    DateTime EndDate,
    Guid OrderId,
    SubscriptionStatus Status);