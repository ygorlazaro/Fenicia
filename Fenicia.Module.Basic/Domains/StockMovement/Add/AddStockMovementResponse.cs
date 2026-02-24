using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.Add;

public record AddStockMovementResponse(
    Guid Id,
    Guid ProductId,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    StockMovementType Type,
    Guid? CustomerId,
    Guid? SupplierId);