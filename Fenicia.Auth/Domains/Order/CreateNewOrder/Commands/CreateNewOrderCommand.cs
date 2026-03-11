namespace Fenicia.Auth.Domains.Order.CreateNewOrder.Commands;

public sealed record CreateNewOrderCommand(Guid UserId, Guid CompanyId, List<Guid> Modules);
