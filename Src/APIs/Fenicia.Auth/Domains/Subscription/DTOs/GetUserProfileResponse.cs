namespace Fenicia.Auth.Domains.Subscription.DTOs;

public record GetUserProfileResponse(Guid Id, string Name, string Email, IEnumerable<UserCompanyResponse> Companies, IEnumerable<UserSubscriptionResponse> Subscriptions);
