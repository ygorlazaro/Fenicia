namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
///     Response record containing supplier summary statistics.
/// </summary>
public record SupplierSummaryResponse
{
    /// <summary>
    ///     Total number of suppliers.
    /// </summary>
    public int TotalSuppliers { get; set; }

    /// <summary>
    ///     Total number of products.
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    ///     Total stock value across all suppliers.
    /// </summary>
    public decimal TotalStockValue { get; set; }

    /// <summary>
    ///     Average number of products per supplier.
    /// </summary>
    public decimal AverageProductsPerSupplier { get; set; }
}