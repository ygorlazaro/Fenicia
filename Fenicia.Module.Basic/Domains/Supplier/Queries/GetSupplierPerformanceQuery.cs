namespace Fenicia.Module.Basic.Domains.Supplier.Queries;

/// <summary>
///     Query record for retrieving supplier performance analytics.
/// </summary>
public record GetSupplierPerformanceQuery(
    /// <summary>
    /// Number of days to analyze.
    /// </summary>
    int Days = 90,
    /// <summary>
    /// Number of top suppliers to return.
    /// </summary>
    int TopLimit = 10);