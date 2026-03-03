namespace Fenicia.Auth.Domains.Subscription.GetUserProfile;

public class UserSubscriptionResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<SubscribedModuleResponse> Modules { get; set; } = [];
}