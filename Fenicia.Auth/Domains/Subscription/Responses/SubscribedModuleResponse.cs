namespace Fenicia.Auth.Domains.Subscription.Responses;

public record SubscribedModuleResponse(
    Guid Id,
    string Name,
    string Type,
    DateTime SubscribedAt);