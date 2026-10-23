using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Response DTO containing order details.
/// </summary>
public record OrderResponse(
    [Required] Guid Id,
    [Required][MaxLength(40)] string OrderNumber,
    [Required] Guid UserId,
    [Required] Guid CustomerId,
    [Required][MaxLength(200)] string CustomerName,
    [Range(0, double.MaxValue)] decimal TotalAmount,
    [Range(0, double.MaxValue)] decimal DiscountAmount,
    [Range(0, int.MaxValue)] int TotalQuantity,
    [Required] DateTime SaleDate,
    [Required][MaxLength(200)] string Status,
    [Required] EnumPaymentMethod PaymentMethod,
    [MaxLength(1000)] string? Notes,
    Guid? EmployeeId,
    string? EmployeeName,
    int TotalItems,
    List<OrderDetailResponse>? Details = null) : ICrudItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderResponse"/> class with default values.
    /// </summary>
    public OrderResponse()
        : this(Guid.Empty, string.Empty, Guid.Empty, Guid.Empty, string.Empty, 0, 0, 0, DateTime.MinValue, string.Empty, EnumPaymentMethod.Cash, null, null, null, 0, null)
    {
    }
}