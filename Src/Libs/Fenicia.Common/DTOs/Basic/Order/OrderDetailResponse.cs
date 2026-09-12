using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public record OrderDetailResponse(
    [Required] Guid Id,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    decimal Price,
    decimal DiscountAmount,
    double Quantity,
    decimal Subtotal);