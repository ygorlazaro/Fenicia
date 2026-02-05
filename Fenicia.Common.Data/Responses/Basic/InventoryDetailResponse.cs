namespace Fenicia.Common.Data.Responses.Basic;

public class InventoryDetailResponse
{
    public Guid ProductId { get; set; }

    public double Quantity { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SalesPrice { get; set; }

    public decimal Profit => SalesPrice - CostPrice;

    public string ProductName { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}