namespace Fenicia.Module.Basic.Domains.StockMovement.GetStockMovementDashboard;

public record GetStockMovementDashboardQuery(
    int Days = 30,
    int TopLimit = 10);
