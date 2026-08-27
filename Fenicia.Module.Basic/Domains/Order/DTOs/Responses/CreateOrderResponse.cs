using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

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