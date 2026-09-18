using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public class SupplierBreakdownResponse()
{
    public SupplierBreakdownResponse(
        Guid supplierId,
        string supplierName,
        decimal totalCostValue,
        decimal totalSalesValue,
        double totalQuantity)
        : this()
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        TotalCostValue = totalCostValue;
        TotalSalesValue = totalSalesValue;
        TotalQuantity = totalQuantity;
    }

    [Required]
    public Guid SupplierId { get; set; }

    [Required]
    [MaxLength(200)]
    public string SupplierName { get; set; } = string.Empty;

    public decimal TotalCostValue { get; set; }

    public decimal TotalSalesValue { get; set; }

    public double TotalQuantity { get; set; }
}
