using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record OrderStatusCountResponse([Required] [MaxLength(200)] string Status, int Count, decimal TotalValue);