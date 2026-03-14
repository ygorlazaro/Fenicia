namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
///     Response record containing supplier performance analytics.
/// </summary>
public record SupplierPerformanceResponse
{
    /// <summary>
    ///     Product counts per supplier.
    /// </summary>
    public List<SupplierProductCountResponse> ProductsPerSupplier { get; set; } = [];

    /// <summary>
    ///     Cost comparison for products with multiple suppliers.
    /// </summary>
    public List<SupplierCostComparisonResponse> CostComparison { get; set; } = [];

    /// <summary>
    ///     Recent stock movements from suppliers.
    /// </summary>
    public List<SupplierStockMovementResponse> RecentStockMovements { get; set; } = [];

    /// <summary>
    ///     Summary statistics.
    /// </summary>
    public SupplierSummaryResponse Summary { get; set; } = new();
}