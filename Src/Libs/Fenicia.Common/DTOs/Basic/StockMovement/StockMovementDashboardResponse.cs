namespace Fenicia.Common.DTOs.Basic.StockMovement;

public record StockMovementDashboardResponse
{
    public List<StockMovementHistoryResponse> History { get; set; } = [];

    public List<MonthlyInOutResponse> MonthlyInOut { get; set; } = [];

    public List<TopMovedProductResponse> TopMovedProducts { get; set; } = [];

    public List<StockTurnoverResponse> TurnoverRates { get; set; } = [];
}