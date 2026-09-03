namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record InventoryResponse
{
    public List<InventoryDetailResponse> Items { get; init; } = [];

    public decimal TotalCostPrice { get; init; }

    public decimal TotalSalesPrice { get; set; }

    public double TotalQuantity { get; set; }
}