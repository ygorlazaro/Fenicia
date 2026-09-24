using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing supplier breakdown data.
/// </summary>
/// <param name="supplierId">The unique identifier of the supplier.</param>
/// <param name="supplierName">The name of the supplier.</param>
/// <param name="totalCostValue">The total cost value.</param>
/// <param name="totalSalesValue">The total sales value.</param>
/// <param name="totalQuantity">The total quantity.</param>
public record SupplierBreakdownResponse(
    [Required] Guid SupplierId,
    [Required] [MaxLength(200)] string SupplierName,
    decimal TotalCostValue,
    decimal TotalSalesValue,
    double TotalQuantity);