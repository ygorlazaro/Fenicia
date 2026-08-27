namespace Fenicia.Module.Basic.Domains.OrderDetail.DTOs.Responses;

public record GetOrderDetailsByOrderIdResponse(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    decimal Price,
    decimal DiscountAmount,
    double Quantity,
    decimal Subtotal);
