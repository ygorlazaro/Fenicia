using Fenicia.Common.Enums.Auth;

namespace Fenicia.Module.Basic.Domains.Order;

public record CreateOrderCommand(
    Guid UserId,
    Guid CustomerId,
    DateTime SaleDate,
    OrderStatus Status,
    List<OrderDetailCommand> Details);

public record OrderDetailCommand(
    Guid ProductId,
    decimal Price,
    double Quantity);

public record OrderResponse(
    Guid Id,
    Guid UserId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime SaleDate,
    OrderStatus Status);
