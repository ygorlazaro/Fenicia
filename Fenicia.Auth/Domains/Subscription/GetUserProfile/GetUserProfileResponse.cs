namespace Fenicia.Auth.Domains.Subscription.GetUserProfile;

public class GetUserProfileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<UserCompanyResponse> Companies { get; set; } = [];
    public List<UserSubscriptionResponse> Subscriptions { get; set; } = [];
}