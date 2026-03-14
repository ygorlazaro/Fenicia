namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

/// <summary>
///     Response record for a top moved product in dashboard analytics.
/// </summary>
public record TopMovedProductResponse(
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
    /// Total quantity moved.
    /// </summary>
    double TotalMoved,
    /// <summary>
    /// Total value of movements.
    /// </summary>
    decimal TotalValue,
    /// <summary>
    /// Number of movements.
    /// </summary>
    int MovementCount);