namespace Fenicia.Module.Basic.Domains.Order.Responses;

public record GetOrderByIdResponse(
    Guid Id,
    Guid UserId,
    Guid CustomerId,
    string CustomerName,
    decimal TotalAmount,
    DateTime SaleDate,
    string Status,
    List<OrderDetailResponse> Details);