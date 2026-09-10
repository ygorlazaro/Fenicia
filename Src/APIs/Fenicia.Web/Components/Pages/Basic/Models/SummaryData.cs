namespace Fenicia.Web.Components.Pages.Basic.Models;

public class SummaryData
{
    public int TotalProducts { get; set; }

    public int HealthyProducts { get; set; }

    public int ZeroMovementProducts { get; set; }

    public decimal TotalStockValue { get; set; }

    public decimal ZeroMovementPercentage { get; set; }
}
