using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public record ProfitMarginResponse(
    [Required] Guid ProductId,
    [Required] [MaxLength(200)] string ProductName,
    [Required] [MaxLength(200)] string CategoryName,
    decimal CostPrice,
    decimal SalesPrice,
    decimal ProfitMargin,
    [Required] [MaxLength(200)] string MarginClassification);