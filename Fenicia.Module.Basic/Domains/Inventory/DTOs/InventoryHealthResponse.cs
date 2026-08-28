namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record InventoryHealthResponse
{

    public OverstockAlertResponse OverstockAlert { get; set; } = new();

    public List<ZeroMovementProductResponse> ZeroMovementProducts { get; set; } = [];

    public List<StockValueByCategoryResponse> StockValueByCategory { get; set; } = [];

    public InventoryHealthSummaryResponse Summary { get; set; } = new();
}
