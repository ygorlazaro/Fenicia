using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

public record GetOrderByIdResponse(
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
    string? Notes,
    Guid? EmployeeId = null);
