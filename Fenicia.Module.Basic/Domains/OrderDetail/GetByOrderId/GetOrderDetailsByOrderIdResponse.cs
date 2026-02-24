namespace Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

public record GetOrderDetailsByOrderIdResponse(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    decimal Price,
    double Quantity);