using Fenicia.Auth.Domains.Order.Response;

using MediatR;

namespace Fenicia.Auth.Domains.Order.Command;

/// <summary>
///     Command to create a new module subscription order.
/// </summary>
/// <remarks>
///     Used by <see cref="Handler.CreateNewOrderHandler" /> to process order requests.
///     The user must have a role in the company to place an order.
/// </remarks>
public sealed record CreateNewOrderCommand(Guid UserId, Guid CompanyId, List<Guid> Modules) : IRequest<CreateNewOrderResponse?>;
