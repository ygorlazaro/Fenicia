namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
/// Response model for overstock alerts.
/// </summary>
public record OverstockAlertResponse
{
    /// <summary>Total number of overstock products.</summary>
    public int TotalOverstockProducts { get; set; }
    /// <summary>Total value of overstock products.</summary>
    public decimal TotalOverstockValue { get; set; }
    /// <summary>List of overstock products.</summary>
    public List<OverstockProductResponse> Products { get; set; } = [];
}
