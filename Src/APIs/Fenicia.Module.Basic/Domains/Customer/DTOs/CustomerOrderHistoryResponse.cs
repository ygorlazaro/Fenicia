using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record CustomerOrderHistoryResponse([Required] Guid CustomerId, [Required][MaxLength(200)] string CustomerName, int OrderCount, decimal TotalSpent, int TotalItems, [Required] DateTime FirstOrderDate, [Required] DateTime LastOrderDate, decimal AverageOrderValue);
