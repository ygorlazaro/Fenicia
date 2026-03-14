using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Commands;

/// <summary>
/// Command record for updating an existing stock movement.
/// </summary>
public record UpdateStockMovementCommand(
    /// <summary>
    /// Unique identifier of the stock movement to update.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Updated quantity.
    /// </summary>
    double Quantity,
    /// <summary>
    /// Updated date.
    /// </summary>
    DateTime? Date,
    /// <summary>
    /// Updated price per unit.
    /// </summary>
    decimal Price,
    /// <summary>
    /// Updated type of movement (In or Out).
    /// </summary>
    StockMovementType Type,
    /// <summary>
    /// Updated product ID.
    /// </summary>
    Guid ProductId,
    /// <summary>
    /// Updated customer ID.
    /// </summary>
    Guid? CustomerId,
    /// <summary>
    /// Updated supplier ID.
    /// </summary>
    Guid? SupplierId,
    /// <summary>
    /// Updated employee ID.
    /// </summary>
    Guid? EmployeeId,
    /// <summary>
    /// Updated order ID.
    /// </summary>
    Guid? OrderId,
    /// <summary>
    /// Updated reason for the movement.
    /// </summary>
    string? Reason);
