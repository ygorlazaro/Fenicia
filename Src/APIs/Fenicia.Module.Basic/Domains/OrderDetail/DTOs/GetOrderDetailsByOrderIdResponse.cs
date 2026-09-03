using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.OrderDetail.DTOs;

public record GetOrderDetailsByOrderIdResponse(
    [Required] Guid Id,
    [Required] Guid OrderId,
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    decimal Price,
    decimal DiscountAmount,
    double Quantity,
    decimal Subtotal);