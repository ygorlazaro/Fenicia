namespace Fenicia.Module.Basic.Domains.Order.GetAll;

public record GetAllOrderResponse(
    Guid Id,
    Guid UserId,
    Guid CustomerId,
    string CustomerName,
    decimal TotalAmount,
    DateTime SaleDate,
    string Status,
    int TotalItems);
