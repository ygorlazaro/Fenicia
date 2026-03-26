using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.Responses;

/// <summary>
///     Response containing order summary for list views.
/// </summary>
public record GetAllOrderResponse(
    Guid Id,
    string OrderNumber,
    Guid UserId,
    Guid CustomerId,
    string CustomerName,
    decimal TotalAmount,
    decimal DiscountAmount,
    int TotalQuantity,
    DateTime SaleDate,
    string Status,
    PaymentMethod PaymentMethod,
    int TotalItems,
    Guid? EmployeeId = null,
    string? EmployeeName = null);