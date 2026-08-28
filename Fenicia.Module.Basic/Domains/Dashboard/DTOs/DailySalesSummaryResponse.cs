namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs;

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
