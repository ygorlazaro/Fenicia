namespace Fenicia.Module.Basic.Domains.Inventory.DTOs.Responses;

public record InventoryDashboardResponse
{

    public List<InventoryDashboardItemResponse> LowStockItems { get; set; } = [];

    public int TotalCustomers { get; set; }

    public int TotalEmployees { get; set; }

    public decimal TotalCostValue { get; set; }

    public decimal TotalSalesValue { get; set; }

    public double TotalQuantity { get; set; }

    public decimal ProfitPotential { get; set; }

    public List<CategoryBreakdownResponse> CategoryBreakdown { get; set; } = [];

    public List<SupplierBreakdownResponse> SupplierBreakdown { get; set; } = [];
}