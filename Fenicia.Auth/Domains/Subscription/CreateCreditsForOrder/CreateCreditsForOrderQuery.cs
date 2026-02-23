namespace Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;

public record CreateCreditsForOrderQuery(
    Guid Id,
    Guid CompanyId,
    IEnumerable<CreateCreditsForOrderDetailsQuery> Details);