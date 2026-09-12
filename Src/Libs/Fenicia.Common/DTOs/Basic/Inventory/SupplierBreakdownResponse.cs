using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public record SupplierBreakdownResponse(
    [Required] Guid SupplierId,
    [Required] [MaxLength(200)] string SupplierName,
    decimal TotalCostValue,
    decimal TotalSalesValue,
    double TotalQuantity);