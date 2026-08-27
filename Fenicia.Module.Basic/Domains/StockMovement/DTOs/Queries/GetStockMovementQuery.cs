namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs.Queries;

public record GetStockMovementQuery(DateTime? StartDate, DateTime? EndDate, int Page = 1, int PageSize = 10);
