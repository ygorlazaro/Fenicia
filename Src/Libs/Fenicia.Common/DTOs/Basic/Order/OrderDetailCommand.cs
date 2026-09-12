using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public record OrderDetailCommand(
    [Required] Guid ProductId,
    decimal Price,
    double Quantity,
    decimal DiscountAmount = 0);