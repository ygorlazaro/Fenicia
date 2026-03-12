namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

public record InventoryHealthSummaryResponse
{
    public int TotalProducts { get; set; }
    public int HealthyProducts { get; set; }
    public int OverstockProducts { get; set; }
    public int ZeroMovementProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public decimal OverstockPercentage { get; set; }
    public decimal ZeroMovementPercentage { get; set; }
}
