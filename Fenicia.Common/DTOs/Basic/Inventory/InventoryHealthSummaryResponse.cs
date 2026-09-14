namespace Fenicia.Common.DTOs.Basic.Inventory;

public record InventoryHealthSummaryResponse
{
    public int TotalProducts { get; set; }

    public int HealthyProducts { get; set; }

    public int ZeroMovementProducts { get; set; }

    public decimal TotalStockValue { get; set; }

    public decimal ZeroMovementPercentage { get; set; }
}