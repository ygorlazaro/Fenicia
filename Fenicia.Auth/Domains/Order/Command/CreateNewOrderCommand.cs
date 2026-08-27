using Fenicia.Auth.Domains.Order.Response;

using MediatR;

namespace Fenicia.Auth.Domains.Order.Command;

public sealed record CreateNewOrderCommand(Guid UserId, Guid CompanyId, List<Guid> Modules) : IRequest<CreateNewOrderResponse?>;
