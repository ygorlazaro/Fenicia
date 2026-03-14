namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

/// <summary>
/// Response record containing stock movement dashboard analytics.
/// </summary>
public record StockMovementDashboardResponse
{
    /// <summary>
    /// Recent stock movement history.
    /// </summary>
    public List<StockMovementHistoryResponse> History { get; set; } = [];
    /// <summary>
    /// Monthly totals for stock in/out.
    /// </summary>
    public List<MonthlyInOutResponse> MonthlyInOut { get; set; } = [];
    /// <summary>
    /// Top moved products by quantity.
    /// </summary>
    public List<TopMovedProductResponse> TopMovedProducts { get; set; } = [];
    /// <summary>
    /// Stock turnover rates by product.
    /// </summary>
    public List<StockTurnoverResponse> TurnoverRates { get; set; } = [];
}