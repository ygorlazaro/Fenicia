namespace Fenicia.Auth.Domains.Order.CreateNewOrder;

public sealed record CreateNewOrderCommand(Guid UserId, Guid CompanyId, List<Guid> Modules);