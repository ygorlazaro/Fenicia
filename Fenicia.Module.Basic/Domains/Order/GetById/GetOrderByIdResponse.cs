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