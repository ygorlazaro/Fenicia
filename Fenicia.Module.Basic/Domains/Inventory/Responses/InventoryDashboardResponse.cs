namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model for inventory dashboard with overview metrics.
/// </summary>
public record InventoryDashboardResponse
{
    /// <summary>List of low stock items.</summary>
    public List<InventoryDashboardItemResponse> LowStockItems { get; set; } = [];

    /// <summary>Total number of customers.</summary>
    public int TotalCustomers { get; set; }

    /// <summary>Total number of employees.</summary>
    public int TotalEmployees { get; set; }

    /// <summary>Total cost value of inventory.</summary>
    public decimal TotalCostValue { get; set; }

    /// <summary>Total sales value of inventory.</summary>
    public decimal TotalSalesValue { get; set; }

    /// <summary>Total quantity of all products.</summary>
    public double TotalQuantity { get; set; }

    /// <summary>Potential profit (sales value - cost value).</summary>
    public decimal ProfitPotential { get; set; }

    /// <summary>Inventory breakdown by category.</summary>
    public List<CategoryBreakdownResponse> CategoryBreakdown { get; set; } = [];

    /// <summary>Inventory breakdown by supplier.</summary>
    public List<SupplierBreakdownResponse> SupplierBreakdown { get; set; } = [];
}