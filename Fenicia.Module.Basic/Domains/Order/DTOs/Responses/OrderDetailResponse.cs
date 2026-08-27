namespace Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

public record OrderDetailResponse(Guid Id, Guid ProductId, string ProductName, decimal Price, decimal DiscountAmount, double Quantity, decimal Subtotal);