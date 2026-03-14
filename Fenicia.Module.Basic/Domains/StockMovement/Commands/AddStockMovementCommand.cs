using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Commands;

/// <summary>
/// Command record for creating a new stock movement.
/// </summary>
public record AddStockMovementCommand(
    /// <summary>
    /// Unique identifier for the stock movement.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Quantity of the movement.
    /// </summary>
    double Quantity,
    /// <summary>
    /// Date of the movement.
    /// </summary>
    DateTime? Date,
    /// <summary>
    /// Price per unit.
    /// </summary>
    decimal Price,
    /// <summary>
    /// Type of movement (In or Out).
    /// </summary>
    StockMovementType Type,
    /// <summary>
    /// Product ID.
    /// </summary>
    Guid ProductId,
    /// <summary>
    /// Customer ID (optional, for sales).
    /// </summary>
    Guid? CustomerId,
    /// <summary>
    /// Supplier ID (optional, for purchases).
    /// </summary>
    Guid? SupplierId,
    /// <summary>
    /// Employee ID (optional).
    /// </summary>
    Guid? EmployeeId,
    /// <summary>
    /// Order ID (optional).
    /// </summary>
    Guid? OrderId,
    /// <summary>
    /// Reason for the movement.
    /// </summary>
    string? Reason);
