using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement.GetMovement;

public record GetStockMovementResponse(
    Guid Id,
    Guid ProductId,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    StockMovementType Type,
    Guid? CustomerId,
    Guid? SupplierId);