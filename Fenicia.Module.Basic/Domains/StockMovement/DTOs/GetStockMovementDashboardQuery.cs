namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record GetStockMovementDashboardQuery(

    int Days = 30,

    int TopLimit = 10);
