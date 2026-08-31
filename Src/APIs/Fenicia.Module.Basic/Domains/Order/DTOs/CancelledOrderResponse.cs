using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record CancelledOrderResponse([Required] Guid OrderId, [Required][MaxLength(200)] string CustomerName, decimal TotalAmount, [Required] DateTime SaleDate, int TotalItems, string? CancelledReason);
