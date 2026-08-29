namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record SupplierPerformanceResponse
{
    public List<SupplierProductCountResponse> ProductsPerSupplier { get; set; } = [];

    public List<SupplierCostComparisonResponse> CostComparison { get; set; } = [];

    public List<SupplierStockMovementResponse> RecentStockMovements { get; set; } = [];

    public SupplierSummaryResponse Summary { get; set; } = new();
}
