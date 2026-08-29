namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record GetStockMovementQuery(DateTime? StartDate, DateTime? EndDate, int Page = 1, int PerPage = 10);
