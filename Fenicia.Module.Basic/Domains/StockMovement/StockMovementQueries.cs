using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public record GetStockMovementQuery(DateTime StartDate, DateTime EndDate, int Page = 1, int PerPage = 10);

public record AddStockMovementCommand(
    Guid Id,
    int Quantity,
    DateTime? Date,
    decimal Price,
    StockMovementType Type,
    Guid ProductId,
    Guid? CustomerId,
    Guid? SupplierId);

public record UpdateStockMovementCommand(
    Guid Id,
    int Quantity,
    DateTime? Date,
    decimal Price,
    StockMovementType Type,
    Guid ProductId,
    Guid? CustomerId,
    Guid? SupplierId);

public record StockMovementResponse(
    Guid Id,
    Guid ProductId,
    double Quantity,
    DateTime? Date,
    decimal? Price,
    StockMovementType Type,
    Guid? CustomerId,
    Guid? SupplierId);
