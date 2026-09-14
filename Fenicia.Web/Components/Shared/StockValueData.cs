namespace Fenicia.Web.Components.Shared;

public class StockValueData
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int ProductCount { get; set; }

    public decimal TotalStockValue { get; set; }

    public double Percentage { get; set; }
}
