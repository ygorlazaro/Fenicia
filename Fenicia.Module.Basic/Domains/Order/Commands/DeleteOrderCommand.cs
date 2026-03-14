namespace Fenicia.Module.Basic.Domains.Order.Commands;

/// <summary>
///     Command to delete (soft-delete) an order.
/// </summary>
public record DeleteOrderCommand(Guid Id);