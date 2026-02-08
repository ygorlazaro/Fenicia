namespace Fenicia.Common.Data.Responses.Basic;

public class InventoryResponse
{
    public decimal TotalCostPrice { get; set; }

    public decimal TotalSalesPrice { get; set; }

    public decimal TotalProfit => this.TotalSalesPrice - this.TotalCostPrice;

    public double TotalQuantity { get; set; }

    public List<InventoryDetailResponse> Items { get; set; } = [];
}
