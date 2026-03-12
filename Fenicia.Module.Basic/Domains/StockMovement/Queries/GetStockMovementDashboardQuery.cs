namespace Fenicia.Module.Basic.Domains.StockMovement.Queries;

public record GetStockMovementDashboardQuery(
    int Days = 30,
    int TopLimit = 10);
