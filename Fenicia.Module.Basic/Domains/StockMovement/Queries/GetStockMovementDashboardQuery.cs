namespace Fenicia.Module.Basic.Domains.StockMovement.Queries;

/// <summary>
///     Query record for retrieving stock movement dashboard analytics.
/// </summary>
public record GetStockMovementDashboardQuery(
    /// <summary>
    /// Number of days to analyze.
    /// </summary>
    int Days = 30,
    /// <summary>
    /// Number of top products to return.
    /// </summary>
    int TopLimit = 10);