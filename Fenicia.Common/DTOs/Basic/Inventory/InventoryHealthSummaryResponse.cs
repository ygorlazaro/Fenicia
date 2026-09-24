namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing inventory health summary metrics.
/// </summary>
public class InventoryHealthSummaryResponse
{
    /// <summary>
    /// Gets or sets the total number of products.
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Gets or sets the number of healthy products.
    /// </summary>
    public int HealthyProducts { get; set; }

    /// <summary>
    /// Gets or sets the number of zero movement products.
    /// </summary>
    public int ZeroMovementProducts { get; set; }

    /// <summary>
    /// Gets or sets the total stock value.
    /// </summary>
    public decimal TotalStockValue { get; set; }

    /// <summary>
    /// Gets or sets the zero movement percentage.
    /// </summary>
    public decimal ZeroMovementPercentage { get; set; }
}