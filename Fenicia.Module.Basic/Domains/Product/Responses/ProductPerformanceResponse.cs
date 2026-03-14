namespace Fenicia.Module.Basic.Domains.Product.Responses;

/// <summary>
///     Response record containing product performance metrics.
/// </summary>
public record ProductPerformanceResponse
{
    /// <summary>
    ///     List of best-selling products.
    /// </summary>
    public List<BestSellingProductResponse> BestSellingProducts { get; set; } = [];

    /// <summary>
    ///     List of worst-selling products.
    /// </summary>
    public List<WorstSellingProductResponse> WorstSellingProducts { get; set; } = [];

    /// <summary>
    ///     List of products with profit margin analysis.
    /// </summary>
    public List<ProfitMarginResponse> ProfitMargins { get; set; } = [];

    /// <summary>
    ///     List of products that have never been sold.
    /// </summary>
    public List<NeverSoldProductResponse> NeverSoldProducts { get; set; } = [];
}