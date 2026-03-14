namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model for inventory health analysis.
/// </summary>
public record InventoryHealthResponse
{
    /// <summary>Overstock alert with products and totals.</summary>
    public OverstockAlertResponse OverstockAlert { get; set; } = new();

    /// <summary>List of products with no movement.</summary>
    public List<ZeroMovementProductResponse> ZeroMovementProducts { get; set; } = [];

    /// <summary>Stock value breakdown by category.</summary>
    public List<StockValueByCategoryResponse> StockValueByCategory { get; set; } = [];

    /// <summary>Health summary metrics.</summary>
    public InventoryHealthSummaryResponse Summary { get; set; } = new();
}