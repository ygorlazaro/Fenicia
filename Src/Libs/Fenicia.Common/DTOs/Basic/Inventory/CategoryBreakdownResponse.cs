using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public record CategoryBreakdownResponse(
    [Required] Guid CategoryId,
    [Required] [MaxLength(200)] string CategoryName,
    decimal TotalCostValue,
    decimal TotalSalesValue,
    double TotalQuantity);