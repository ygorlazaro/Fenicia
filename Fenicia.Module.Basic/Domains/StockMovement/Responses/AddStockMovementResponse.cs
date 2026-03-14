using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

/// <summary>
/// Response record for a newly created stock movement.
/// </summary>
public record AddStockMovementResponse(
    /// <summary>
    /// Unique identifier of the stock movement.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Product ID.
    /// </summary>
    Guid ProductId,
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
    decimal? Price,
    /// <summary>
    /// Type of movement (In or Out).
    /// </summary>
    StockMovementType Type,
    /// <summary>
    /// Customer ID (optional).
    /// </summary>
    Guid? CustomerId,
    /// <summary>
    /// Supplier ID (optional).
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
