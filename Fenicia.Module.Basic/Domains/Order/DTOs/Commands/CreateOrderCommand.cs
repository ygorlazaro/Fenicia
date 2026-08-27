using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Order.DTOs.Commands;

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
