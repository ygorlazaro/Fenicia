namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model for inventory health summary metrics.
/// </summary>
public record InventoryHealthSummaryResponse
{
    /// <summary>Total number of products with stock.</summary>
    public int TotalProducts { get; set; }

    /// <summary>Number of healthy products.</summary>
    public int HealthyProducts { get; set; }

    /// <summary>Number of overstock products.</summary>
    public int OverstockProducts { get; set; }

    /// <summary>Number of products with no movement.</summary>
    public int ZeroMovementProducts { get; set; }

    /// <summary>Total stock value.</summary>
    public decimal TotalStockValue { get; set; }

    /// <summary>Percentage of overstock products.</summary>
    public decimal OverstockPercentage { get; set; }

    /// <summary>Percentage of products with no movement.</summary>
    public decimal ZeroMovementPercentage { get; set; }
}