namespace Fenicia.Web.Components.Pages.Basic.Models;

public class HealthData
{
    public List<ZeroMovementItem> ZeroMovementProducts { get; set; } = [];

    public List<StockValueData> StockValueByCategory { get; set; } = [];

    public SummaryData Summary { get; set; } = new();
}
