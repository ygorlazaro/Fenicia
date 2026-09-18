using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class SupplierCostComparisonResponse()
{
    public SupplierCostComparisonResponse(string productName, List<ProductSupplierPriceResponse> suppliers)
        : this()
    {
        ProductName = productName;
        Suppliers = suppliers;
    }

    [Required]
    [MaxLength(50)]
    public string ProductName { get; set; } = string.Empty;

    public List<ProductSupplierPriceResponse> Suppliers { get; set; } = [];
}
