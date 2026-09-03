using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record OrderDetailResponse(
    [Required] Guid Id,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    decimal Price,
    decimal DiscountAmount,
    double Quantity,
    decimal Subtotal);