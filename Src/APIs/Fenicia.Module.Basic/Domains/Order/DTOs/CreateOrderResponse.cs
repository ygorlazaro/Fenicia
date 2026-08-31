using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record CreateOrderResponse(
    [Required] Guid Id,
    [Required][MaxLength(200)] string OrderNumber,
    [Required] Guid UserId,
    [Required] Guid CustomerId,
    decimal TotalAmount,
    decimal DiscountAmount,
    int TotalQuantity,
    [Required] DateTime SaleDate,
    [Required] OrderStatus Status,
    [Required] PaymentMethod PaymentMethod,
    string? Notes = null,
    Guid? EmployeeId = null);
