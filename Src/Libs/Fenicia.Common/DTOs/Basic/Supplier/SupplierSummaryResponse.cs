namespace Fenicia.Common.DTOs.Basic.Supplier;

public record SupplierSummaryResponse
{
    public int TotalSuppliers { get; set; }

    public int TotalProducts { get; set; }

    public decimal TotalStockValue { get; set; }

    public decimal AverageProductsPerSupplier { get; set; }
}