namespace Fenicia.Module.Basic.Domains.Product.GetProductPerformance;

public record ProductPerformanceResponse
{
    public List<BestSellingProductResponse> BestSellingProducts { get; set; } = [];
    public List<WorstSellingProductResponse> WorstSellingProducts { get; set; } = [];
    public List<ProfitMarginResponse> ProfitMargins { get; set; } = [];
    public List<NeverSoldProductResponse> NeverSoldProducts { get; set; } = [];
}

public record BestSellingProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double TotalQuantitySold,
    decimal TotalRevenue,
    int OrderCount,
    decimal AveragePrice);

public record WorstSellingProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double TotalQuantitySold,
    decimal TotalRevenue,
    int OrderCount,
    double CurrentStock,
    decimal CostValue);

public record ProfitMarginResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    decimal CostPrice,
    decimal SalesPrice,
    decimal ProfitMargin,
    string MarginClassification);

public record NeverSoldProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    string? SupplierName,
    double CurrentStock,
    decimal CostValue,
    DateTime? LastStockMovement);
