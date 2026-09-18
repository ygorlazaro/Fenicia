using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class ProductSupplierPriceResponse()
{
    public ProductSupplierPriceResponse(
        Guid supplierId,
        string supplierName,
        decimal costPrice,
        decimal salesPrice,
        decimal profitMargin)
        : this()
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        CostPrice = costPrice;
        SalesPrice = salesPrice;
        ProfitMargin = profitMargin;
    }

    [Required]
    public Guid SupplierId { get; set; }

    [Required]
    [MaxLength(50)]
    public string SupplierName { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }

    public decimal SalesPrice { get; set; }

    public decimal ProfitMargin { get; set; }
}
