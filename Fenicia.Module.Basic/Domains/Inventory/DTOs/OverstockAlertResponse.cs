namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record OverstockAlertResponse
{

    public int TotalOverstockProducts { get; set; }

    public decimal TotalOverstockValue { get; set; }

    public List<OverstockProductResponse> Products { get; set; } = [];
}
