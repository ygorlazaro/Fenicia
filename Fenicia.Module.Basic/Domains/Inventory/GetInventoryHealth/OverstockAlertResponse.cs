namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryHealth;

public record OverstockAlertResponse
{
    public int TotalOverstockProducts { get; set; }
    public decimal TotalOverstockValue { get; set; }
    public List<OverstockProductResponse> Products { get; set; } = [];
}
