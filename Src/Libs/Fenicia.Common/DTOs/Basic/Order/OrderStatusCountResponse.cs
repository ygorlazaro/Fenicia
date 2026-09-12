using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public record OrderStatusCountResponse([Required] [MaxLength(200)] string Status, int Count, decimal TotalValue);