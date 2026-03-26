using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
///     Response containing the created order information.
/// </summary>
public record CreateOrderResponse(
    Guid Id,
    string OrderNumber,
    Guid UserId,
    Guid CustomerId,
    decimal TotalAmount,
    decimal DiscountAmount,
    int TotalQuantity,
    DateTime SaleDate,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    string? Notes = null,
    Guid? EmployeeId = null);