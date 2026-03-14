namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

/// <summary>
/// Response record for stock turnover rate analysis.
/// </summary>
public record StockTurnoverResponse(
    /// <summary>
    /// Product ID.
    /// </summary>
    Guid ProductId,
    /// <summary>
    /// Product name.
    /// </summary>
    string ProductName,
    /// <summary>
    /// Category name.
    /// </summary>
    string CategoryName,
    /// <summary>
    /// Current stock quantity.
    /// </summary>
    double CurrentStock,
    /// <summary>
    /// Total quantity sold.
    /// </summary>
    double TotalSold,
    /// <summary>
    /// Turnover rate (sold/stock).
    /// </summary>
    double TurnoverRate,
    /// <summary>
    /// Classification of turnover rate (High, Medium, Low, Very Low).
    /// </summary>
    string TurnoverClassification);
