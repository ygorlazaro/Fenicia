using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.Commands;

/// <summary>
///     Command to create a new product order.
/// </summary>
public record CreateOrderCommand(
    Guid UserId,
    Guid CustomerId,
    DateTime SaleDate,
    OrderStatus Status,
    List<OrderDetailCommand> Details,
    PaymentMethod PaymentMethod,
    Guid? EmployeeId = null,
    string? Notes = null,
    decimal DiscountAmount = 0);