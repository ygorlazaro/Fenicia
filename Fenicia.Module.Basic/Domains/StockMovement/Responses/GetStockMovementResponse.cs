using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

/// <summary>
/// Response record for a stock movement in a list.
/// </summary>
public record GetStockMovementResponse(
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
    /// Customer name (optional).
    /// </summary>
    string? CustomerName,
    /// <summary>
    /// Supplier ID (optional).
    /// </summary>
    Guid? SupplierId,
    /// <summary>
    /// Supplier name (optional).
    /// </summary>
    string? SupplierName,
    /// <summary>
    /// Employee ID (optional).
    /// </summary>
    Guid? EmployeeId,
    /// <summary>
    /// Employee name (optional).
    /// </summary>
    string? EmployeeName,
    /// <summary>
    /// Order ID (optional).
    /// </summary>
    Guid? OrderId,
    /// <summary>
    /// Reason for the movement.
    /// </summary>
    string? Reason);
