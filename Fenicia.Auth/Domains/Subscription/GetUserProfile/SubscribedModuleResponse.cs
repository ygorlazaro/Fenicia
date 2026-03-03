namespace Fenicia.Auth.Domains.Subscription.GetUserProfile;

public class SubscribedModuleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime SubscribedAt { get; set; }
}