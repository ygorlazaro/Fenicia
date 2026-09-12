namespace Fenicia.Web.Components.Shared;

public class HealthData
{
    public List<ZeroMovementItem> ZeroMovementProducts { get; set; } = [];

    public List<StockValueData> StockValueByCategory { get; set; } = [];

    public SummaryData Summary { get; set; } = new();
}
