namespace Fenicia.Common.DTOs.Basic.Supplier;

public class SupplierPerformanceResponse
{
    public List<SupplierProductCountResponse> ProductsPerSupplier { get; set; } = [];

    public List<SupplierCostComparisonResponse> CostComparison { get; set; } = [];

    public List<SupplierStockMovementResponse> RecentStockMovements { get; set; } = [];

    public SupplierSummaryResponse Summary { get; set; } = new();
}
