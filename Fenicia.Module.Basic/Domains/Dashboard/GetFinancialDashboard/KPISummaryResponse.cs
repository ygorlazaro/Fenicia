namespace Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

public record KPISummaryResponse
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal TotalStockValue { get; set; }
}
