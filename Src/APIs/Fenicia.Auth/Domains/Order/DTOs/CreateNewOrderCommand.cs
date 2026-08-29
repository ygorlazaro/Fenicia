namespace Fenicia.Auth.Domains.Order.DTOs;

public sealed record CreateNewOrderCommand(Guid UserId, Guid CompanyId, List<Guid> Modules);
