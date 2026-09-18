using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class SupplierProductCountResponse()
{
    public SupplierProductCountResponse(
        Guid supplierId,
        string supplierName,
        int productCount,
        decimal totalStockValue,
        decimal totalRevenue)
        : this()
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        ProductCount = productCount;
        TotalStockValue = totalStockValue;
        TotalRevenue = totalRevenue;
    }

    [Required]
    public Guid SupplierId { get; set; }

    [Required]
    [MaxLength(50)]
    public string SupplierName { get; set; } = string.Empty;

    public int ProductCount { get; set; }

    public decimal TotalStockValue { get; set; }

    public decimal TotalRevenue { get; set; }
}
