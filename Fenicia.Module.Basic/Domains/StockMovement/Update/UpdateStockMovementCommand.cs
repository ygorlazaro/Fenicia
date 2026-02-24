using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Update;

public record UpdateStockMovementCommand(
    Guid Id,
    int Quantity,
    DateTime? Date,
    decimal Price,
    StockMovementType Type,
    Guid ProductId,
    Guid? CustomerId,
    Guid? SupplierId);