namespace Fenicia.Module.Basic.Domains.OrderDetail.Responses;

/// <summary>
/// Represents a single order detail item containing product information, price, and quantity.
/// </summary>
public record GetOrderDetailsByOrderIdResponse(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    decimal Price,
    double Quantity);
