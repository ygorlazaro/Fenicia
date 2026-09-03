using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record CustomerRecentOrdersResponse(
    [Required] Guid OrderId,
    [Required] Guid CustomerId,
    [Required] [MaxLength(200)] string CustomerName,
    decimal TotalAmount,
    [Required] DateTime SaleDate,
    [Required] [MaxLength(200)] string Status,
    int TotalItems);