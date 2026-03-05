namespace Fenicia.Module.Basic.Domains.Order.GetById;

public record GetOrderByIdResponse(
    Guid Id,
    Guid UserId,
    Guid CustomerId,
    string CustomerName,
    decimal TotalAmount,
    DateTime SaleDate,
    string Status,
    List<OrderDetailResponse> Details);

public record OrderDetailResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Price,
    double Quantity,
    decimal Subtotal);
