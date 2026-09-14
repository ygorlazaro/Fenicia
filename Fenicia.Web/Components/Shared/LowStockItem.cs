namespace Fenicia.Web.Components.Shared;

public class LowStockItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public decimal? CostPrice { get; set; }

    public decimal SalesPrice { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}
