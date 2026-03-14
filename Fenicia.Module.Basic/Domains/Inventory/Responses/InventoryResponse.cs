namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model containing inventory data with pagination.
/// </summary>
public class InventoryResponse
{
    /// <summary>List of inventory items.</summary>
    public List<InventoryDetailResponse> Items { get; set; } = [];

    /// <summary>Total cost price of all products.</summary>
    public decimal TotalCostPrice { get; set; }

    /// <summary>Total sales price of all products.</summary>
    public decimal TotalSalesPrice { get; set; }

    /// <summary>Total quantity of all products.</summary>
    public double TotalQuantity { get; set; }
}