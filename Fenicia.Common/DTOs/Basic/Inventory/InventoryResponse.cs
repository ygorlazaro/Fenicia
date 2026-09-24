namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing inventory details with pagination.
/// </summary>
public class InventoryResponse
{
    /// <summary>
    /// Gets or sets the list of inventory items.
    /// </summary>
    public List<InventoryDetailResponse> Items { get; init; } = [];

    /// <summary>
    /// Gets or sets the total cost price of all items.
    /// </summary>
    public decimal TotalCostPrice { get; init; }

    /// <summary>
    /// Gets or sets the total sales price of all items.
    /// </summary>
    public decimal TotalSalesPrice { get; set; }

    /// <summary>
    /// Gets or sets the total quantity of all items.
    /// </summary>
    public double TotalQuantity { get; set; }
}