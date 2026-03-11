namespace Fenicia.Auth.Domains.Subscription.Responses;

public record GetUserProfileResponse(Guid Id, string Name, string Email, IEnumerable<UserCompanyResponse> Companies, IEnumerable<UserSubscriptionResponse> Subscriptions);