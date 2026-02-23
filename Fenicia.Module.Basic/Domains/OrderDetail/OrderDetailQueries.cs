namespace Fenicia.Module.Basic.Domains.OrderDetail;

public record GetOrderDetailsByOrderIdQuery(Guid OrderId);

public record OrderDetailResponse(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    decimal Price,
    double Quantity);
