using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public record SalesTrendResponse(
    [Required] [MaxLength(200)] string Period,
    [Required] DateTime Date,
    int OrderCount,
    decimal TotalValue,
    int TotalItems);