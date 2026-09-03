using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record CategoryBreakdownResponse(
    [Required] Guid CategoryId,
    [Required] [MaxLength(200)] string CategoryName,
    decimal TotalCostValue,
    decimal TotalSalesValue,
    double TotalQuantity);