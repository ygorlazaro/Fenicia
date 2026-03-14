namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

/// <summary>
/// Response record for a stock movement history item.
/// </summary>
public record StockMovementHistoryResponse(
    /// <summary>
    /// Unique identifier of the stock movement.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Product ID.
    /// </summary>
    Guid ProductId,
    /// <summary>
    /// Product name.
    /// </summary>
    string ProductName,
    /// <summary>
    /// Quantity of the movement.
    /// </summary>
    double Quantity,
    /// <summary>
    /// Date of the movement.
    /// </summary>
    DateTime Date,
    /// <summary>
    /// Price per unit.
    /// </summary>
    decimal Price,
    /// <summary>
    /// Type of movement (In or Out).
    /// </summary>
    string Type,
    /// <summary>
    /// Reason for the movement.
    /// </summary>
    string? Reason,
    /// <summary>
    /// Customer name (optional).
    /// </summary>
    string? CustomerName,
    /// <summary>
    /// Supplier name (optional).
    /// </summary>
    string? SupplierName);
