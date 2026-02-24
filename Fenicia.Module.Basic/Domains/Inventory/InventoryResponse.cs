namespace Fenicia.Module.Basic.Domains.Inventory;

public class InventoryResponse
{
    public List<InventoryDetailResponse> Items { get; set; } = [];
    public decimal TotalCostPrice { get; set; }
    public decimal TotalSalesPrice { get; set; }
    public double TotalQuantity { get; set; }
}