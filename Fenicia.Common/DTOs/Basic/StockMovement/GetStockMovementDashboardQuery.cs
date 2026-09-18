namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class GetStockMovementDashboardQuery()
{
    public GetStockMovementDashboardQuery(int days = 30, int topLimit = 10)
        : this()
    {
        Days = days;
        TopLimit = topLimit;
    }

    public int Days { get; set; } = 30;

    public int TopLimit { get; set; } = 10;
}
