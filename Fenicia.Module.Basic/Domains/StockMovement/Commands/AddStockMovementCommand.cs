using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Commands;

public record AddStockMovementCommand(
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
