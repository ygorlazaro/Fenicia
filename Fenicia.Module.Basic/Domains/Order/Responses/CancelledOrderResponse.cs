namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
///     Response containing cancelled order information.
/// </summary>
public record CancelledOrderResponse(Guid OrderId, string CustomerName, decimal TotalAmount, DateTime SaleDate, int TotalItems, string? CancelledReason);