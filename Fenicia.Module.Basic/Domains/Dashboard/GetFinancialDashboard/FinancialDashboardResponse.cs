namespace Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

public record FinancialDashboardResponse
{
    public KPISummaryResponse KPI { get; set; } = new();
    public List<RevenueVsCostResponse> RevenueVsCost { get; set; } = [];
    public List<ProfitMarginTrendResponse> ProfitMarginTrend { get; set; } = [];
    public AccountsReceivableResponse AccountsReceivable { get; set; } = new();
    public DailySalesSummaryResponse DailySales { get; set; } = new();
}

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

public record RevenueVsCostResponse(
    string Period,
    DateTime Date,
    decimal Revenue,
    decimal Cost,
    decimal Profit);

public record ProfitMarginTrendResponse(
    string Period,
    DateTime Date,
    decimal MarginPercentage,
    string Trend);

public record AccountsReceivableResponse
{
    public decimal TotalPending { get; set; }
    public int PendingOrdersCount { get; set; }
    public decimal TotalApproved { get; set; }
    public int ApprovedOrdersCount { get; set; }
}

public record DailySalesSummaryResponse
{
    public decimal TodayRevenue { get; set; }
    public int TodayOrders { get; set; }
    public decimal WeekRevenue { get; set; }
    public int WeekOrders { get; set; }
    public decimal MonthRevenue { get; set; }
    public int MonthOrders { get; set; }
    public decimal PreviousMonthRevenue { get; set; }
    public decimal GrowthPercentage { get; set; }
}
