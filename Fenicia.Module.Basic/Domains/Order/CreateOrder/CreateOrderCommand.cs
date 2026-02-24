using Fenicia.Common.Enums.Auth;

namespace Fenicia.Module.Basic.Domains.Order.CreateOrder;

public record CreateOrderCommand(
    Guid UserId,
    Guid CustomerId,
    DateTime SaleDate,
    OrderStatus Status,
    List<OrderDetailCommand> Details);