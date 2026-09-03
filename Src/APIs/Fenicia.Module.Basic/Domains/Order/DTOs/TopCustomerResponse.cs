using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record TopCustomerResponse(
    [Required] Guid CustomerId,
    [Required] [MaxLength(200)] string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems);