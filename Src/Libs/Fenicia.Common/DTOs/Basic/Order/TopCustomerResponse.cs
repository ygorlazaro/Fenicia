using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public record TopCustomerResponse(
    [Required] Guid CustomerId,
    [Required] [MaxLength(200)] string CustomerName,
    int OrderCount,
    decimal TotalSpent,
    int TotalItems);