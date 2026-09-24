namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Response DTO containing inventory dashboard data.
/// </summary>
public class InventoryDashboardResponse
{
    /// <summary>
    /// Gets or sets the low stock items.
    /// </summary>
    public List<InventoryDetailResponse> LowStockItems { get; set; } = [];

    /// <summary>
    /// Gets or sets the total number of customers.
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// Gets or sets the total number of employees.
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Gets or sets the total cost value.
    /// </summary>
    public decimal TotalCostValue { get; set; }

    /// <summary>
    /// Gets or sets the total sales value.
    /// </summary>
    public decimal TotalSalesValue { get; set; }

    /// <summary>
    /// Gets or sets the total quantity.
    /// </summary>
    public double TotalQuantity { get; set; }

    /// <summary>
    /// Gets or sets the profit potential.
    /// </summary>
    public decimal ProfitPotential { get; set; }

    /// <summary>
    /// Gets or sets the category breakdown.
    /// </summary>
    public List<CategoryBreakdownResponse> CategoryBreakdown { get; set; } = [];

    /// <summary>
    /// Gets or sets the supplier breakdown.
    /// </summary>
    public List<SupplierBreakdownResponse> SupplierBreakdown { get; set; } = [];
}