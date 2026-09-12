using System.ComponentModel.DataAnnotations;
using Fenicia.Common;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

public record GetAllOrderResponse([Required] Guid Id,
    [Required] [MaxLength(200)] string OrderNumber,
    [Required] Guid UserId,
    [Required] Guid CustomerId,
    [Required] [MaxLength(200)] string CustomerName,
    decimal TotalAmount,
    decimal DiscountAmount,
    int TotalQuantity,
    [Required] DateTime SaleDate,
    [Required] [MaxLength(200)] string Status,
    [Required] PaymentMethod PaymentMethod,
    int TotalItems,
    Guid? EmployeeId = null,
    string? EmployeeName = null) : ICrudItem;