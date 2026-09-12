using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public record GetStockMovementQuery(
    DateTime? StartDate,
    DateTime? EndDate,
    StockMovementType? Type = null,
    int Page = 1,
    int PerPage = 10,
    string? Query = null);
