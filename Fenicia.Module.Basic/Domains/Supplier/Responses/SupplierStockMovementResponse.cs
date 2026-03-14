namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
/// Response record for a stock movement associated with a supplier.
/// </summary>
public record SupplierStockMovementResponse(
    /// <summary>
    /// Movement ID.
    /// </summary>
    Guid MovementId,
    /// <summary>
    /// Product ID.
    /// </summary>
    Guid ProductId,
    /// <summary>
    /// Product name.
    /// </summary>
    string ProductName,
    /// <summary>
    /// Quantity moved.
    /// </summary>
    double Quantity,
    /// <summary>
    /// Price per unit.
    /// </summary>
    decimal Price,
    /// <summary>
    /// Date of the movement.
    /// </summary>
    DateTime Date,
    /// <summary>
    /// Type of movement (In or Out).
    /// </summary>
    string MovementType);
