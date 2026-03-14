namespace Fenicia.Auth.Domains.Order.CreateNewOrder.Commands;

/// <summary>
///     Command to create a new module subscription order.
/// </summary>
/// <remarks>
///     Used by <see cref="Handlers.CreateNewOrderHandler" /> to process order requests.
///     The user must have a role in the company to place an order.
/// </remarks>
public sealed record CreateNewOrderCommand(Guid UserId, Guid CompanyId, List<Guid> Modules);