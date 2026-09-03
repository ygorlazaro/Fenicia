using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record OrderDetailCommand(
    [Required] Guid ProductId,
    decimal Price,
    double Quantity,
    decimal DiscountAmount = 0);