namespace Fenicia.Module.Basic.Domains.Supplier.GetSupplierPerformance;

public record SupplierPerformanceResponse
{
    public List<SupplierProductCountResponse> ProductsPerSupplier { get; set; } = [];
    public List<SupplierCostComparisonResponse> CostComparison { get; set; } = [];
    public List<SupplierStockMovementResponse> RecentStockMovements { get; set; } = [];
    public SupplierSummaryResponse Summary { get; set; } = new();
}

public record SupplierProductCountResponse(
    Guid SupplierId,
    string SupplierName,
    int ProductCount,
    decimal TotalStockValue,
    decimal TotalRevenue);

public record SupplierCostComparisonResponse(
    string ProductName,
    List<ProductSupplierPriceResponse> Suppliers);

public record ProductSupplierPriceResponse(
    Guid SupplierId,
    string SupplierName,
    decimal CostPrice,
    decimal SalesPrice,
    decimal ProfitMargin);

public record SupplierStockMovementResponse(
    Guid MovementId,
    Guid ProductId,
    string ProductName,
    double Quantity,
    decimal Price,
    DateTime Date,
    string MovementType);

public record SupplierSummaryResponse
{
    public int TotalSuppliers { get; set; }
    public int TotalProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public decimal AverageProductsPerSupplier { get; set; }
}
