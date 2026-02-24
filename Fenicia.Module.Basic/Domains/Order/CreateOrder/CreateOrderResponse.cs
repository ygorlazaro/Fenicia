using Fenicia.Common.Enums.Auth;

namespace Fenicia.Module.Basic.Domains.Order.CreateOrder;

public record CreateOrderResponse(
    Guid Id,
    Guid UserId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime SaleDate,
    OrderStatus Status);