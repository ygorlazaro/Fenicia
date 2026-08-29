namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record CancelledOrderResponse(Guid OrderId, string CustomerName, decimal TotalAmount, DateTime SaleDate, int TotalItems, string? CancelledReason);
