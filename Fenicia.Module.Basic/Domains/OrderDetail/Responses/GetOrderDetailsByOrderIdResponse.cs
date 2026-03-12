namespace Fenicia.Module.Basic.Domains.OrderDetail.Responses;

public record GetOrderDetailsByOrderIdResponse(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    decimal Price,
    double Quantity);
