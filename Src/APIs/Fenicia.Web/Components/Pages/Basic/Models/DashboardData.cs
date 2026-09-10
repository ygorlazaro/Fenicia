namespace Fenicia.Web.Components.Pages.Basic.Models;

public class DashboardData
{
    public List<LowStockItem> LowStockItems { get; set; } = [];

    public int TotalCustomers { get; set; }

    public int TotalEmployees { get; set; }

    public decimal TotalCostValue { get; set; }

    public decimal TotalSalesValue { get; set; }

    public double TotalQuantity { get; set; }

    public decimal ProfitPotential { get; set; }

    public List<CategoryData> CategoryBreakdown { get; set; } = [];

    public List<SupplierData> SupplierBreakdown { get; set; } = [];
}
