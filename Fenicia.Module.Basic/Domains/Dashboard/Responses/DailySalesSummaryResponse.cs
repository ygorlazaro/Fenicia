namespace Fenicia.Module.Basic.Domains.Dashboard.Responses;

/// <summary>
/// Response model for daily sales summary.
/// Contains sales metrics for today, this week, this month, and comparison with previous month.
/// </summary>
public record DailySalesSummaryResponse
{
    /// <summary>Revenue from today's orders.</summary>
    public decimal TodayRevenue { get; set; }
    /// <summary>Number of orders placed today.</summary>
    public int TodayOrders { get; set; }
    /// <summary>Revenue from this week's orders.</summary>
    public decimal WeekRevenue { get; set; }
    /// <summary>Number of orders placed this week.</summary>
    public int WeekOrders { get; set; }
    /// <summary>Revenue from this month's orders.</summary>
    public decimal MonthRevenue { get; set; }
    /// <summary>Number of orders placed this month.</summary>
    public int MonthOrders { get; set; }
    /// <summary>Revenue from the previous month.</summary>
    public decimal PreviousMonthRevenue { get; set; }
    /// <summary>Month-over-month growth percentage.</summary>
    public decimal GrowthPercentage { get; set; }
}
