namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record OrderDetailResponse(Guid Id, Guid ProductId, string ProductName, decimal Price, decimal DiscountAmount, double Quantity, decimal Subtotal);
