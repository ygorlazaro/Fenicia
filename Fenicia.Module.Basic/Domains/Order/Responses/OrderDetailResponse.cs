namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
///     Response containing a single order item/detail.
/// </summary>
public record OrderDetailResponse(Guid Id, Guid ProductId, string ProductName, decimal Price, double Quantity, decimal Subtotal);