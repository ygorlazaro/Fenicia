using Fenicia.Module.Basic.Domains.StockMovement.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs.Queries;

public record GetStockMovementDashboardQuery(

    int Days = 30,

    int TopLimit = 10);