namespace Fenicia.Auth.Domains.Subscription.Commands;

public record CreateCreditsForOrderCommand(
    Guid Id,
    Guid CompanyId,
    IEnumerable<CreateCreditsForOrderDetailsCommand> Details);