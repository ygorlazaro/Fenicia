namespace Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;

public record CancelledOrderResponse(
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount,
    DateTime SaleDate,
    int TotalItems,
    string? CancelledReason);
