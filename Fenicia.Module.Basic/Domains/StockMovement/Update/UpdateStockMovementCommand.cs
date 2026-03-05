using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Update;

public record UpdateStockMovementCommand(
    Guid Id,
    double Quantity,
    DateTime? Date,
    decimal Price,
    StockMovementType Type,
    Guid ProductId,
    Guid? CustomerId,
    Guid? SupplierId,
    Guid? EmployeeId,
    Guid? OrderId,
    string? Reason);