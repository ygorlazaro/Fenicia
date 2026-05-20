namespace Fenicia.Auth.Domains.Order.Response;

/// <summary>
///     Response containing the ID of a newly created order.
/// </summary>
public record CreateNewOrderResponse(Guid OrderId);
