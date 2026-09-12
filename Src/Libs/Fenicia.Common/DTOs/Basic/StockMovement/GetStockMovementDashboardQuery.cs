namespace Fenicia.Common.DTOs.Basic.StockMovement;

public record GetStockMovementDashboardQuery(
    int Days = 30,
    int TopLimit = 10);