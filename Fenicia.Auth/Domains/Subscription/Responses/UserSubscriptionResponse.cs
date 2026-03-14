using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Subscription.Responses;

public record UserSubscriptionResponse(Guid Id, Guid CompanyId, string CompanyName, SubscriptionStatus Status, DateTime StartDate, DateTime? EndDate);