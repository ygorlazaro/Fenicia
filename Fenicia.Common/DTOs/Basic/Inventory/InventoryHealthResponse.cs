namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing inventory health data.
/// </summary>
public class InventoryHealthResponse
{
    /// <summary>
    /// Gets or sets the zero movement products.
    /// </summary>
    public List<ZeroMovementProductResponse> ZeroMovementProducts { get; set; } = [];

    /// <summary>
    /// Gets or sets the stock value by category.
    /// </summary>
    public List<StockValueByCategoryResponse> StockValueByCategory { get; set; } = [];

    /// <summary>
    /// Gets or sets the inventory health summary.
    /// </summary>
    public InventoryHealthSummaryResponse Summary { get; set; } = new();
}