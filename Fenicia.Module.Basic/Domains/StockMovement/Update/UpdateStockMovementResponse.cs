using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Update;

public record UpdateStockMovementResponse(
    Guid Id,
    Guid ProductId,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    StockMovementType Type,
    Guid? CustomerId,
    Guid? SupplierId);