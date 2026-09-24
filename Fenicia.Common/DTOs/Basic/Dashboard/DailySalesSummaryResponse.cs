namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Response DTO containing daily sales summary data.
/// </summary>
public class DailySalesSummaryResponse
{
    /// <summary>
    /// Gets or sets today's revenue.
    /// </summary>
    public decimal TodayRevenue { get; set; }

    /// <summary>
    /// Gets or sets today's order count.
    /// </summary>
    public int TodayOrders { get; set; }

    /// <summary>
    /// Gets or sets this week's revenue.
    /// </summary>
    public decimal WeekRevenue { get; set; }

    /// <summary>
    /// Gets or sets this week's order count.
    /// </summary>
    public int WeekOrders { get; set; }

    /// <summary>
    /// Gets or sets this month's revenue.
    /// </summary>
    public decimal MonthRevenue { get; set; }

    /// <summary>
    /// Gets or sets this month's order count.
    /// </summary>
    public int MonthOrders { get; set; }

    /// <summary>
    /// Gets or sets the previous month's revenue.
    /// </summary>
    public decimal PreviousMonthRevenue { get; set; }

    /// <summary>
    /// Gets or sets the growth percentage compared to previous month.
    /// </summary>
    public decimal GrowthPercentage { get; set; }
}