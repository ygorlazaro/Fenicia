namespace Fenicia.Module.Basic.Domains.StockMovement.Queries;

/// <summary>
///     Query record for retrieving stock movements with date range filtering.
/// </summary>
public record GetStockMovementQuery(
    /// <summary>
    /// Start date for filtering movements.
    /// </summary>
    DateTime StartDate,
    /// <summary>
    /// End date for filtering movements.
    /// </summary>
    DateTime EndDate,
    /// <summary>
    /// Page number for pagination.
    /// </summary>
    int Page = 1,
    /// <summary>
    /// Number of items per page.
    /// </summary>
    int PerPage = 10);