namespace Fenicia.Common.DTOs.Basic.Inventory;

public class InventoryHealthResponse
{
    public List<ZeroMovementProductResponse> ZeroMovementProducts { get; set; } = [];

    public List<StockValueByCategoryResponse> StockValueByCategory { get; set; } = [];

    public InventoryHealthSummaryResponse Summary { get; set; } = new();
}
