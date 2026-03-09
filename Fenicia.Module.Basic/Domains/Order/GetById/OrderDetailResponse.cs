namespace Fenicia.Module.Basic.Domains.Order.GetById;

public record OrderDetailResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Price,
    double Quantity,
    decimal Subtotal);
