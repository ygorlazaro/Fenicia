using Fenicia.Auth.Domains.Order.DTOs.Responses;

namespace Fenicia.Auth.Domains.Order.DTOs.Commands;

public sealed record CreateNewOrderCommand(Guid UserId, Guid CompanyId, List<Guid> Modules);
