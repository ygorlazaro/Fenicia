using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.OrderDetail;

public record GetOrderDetailsByOrderIdResponse(
    [Required] Guid Id,
    [Required] Guid OrderId,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    decimal Price,
    decimal DiscountAmount,
    double Quantity,
    decimal Subtotal);