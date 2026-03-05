namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryHealth;

public record InventoryHealthResponse
{
    public OverstockAlertResponse OverstockAlert { get; set; } = new();
    public List<ZeroMovementProductResponse> ZeroMovementProducts { get; set; } = [];
    public List<StockValueByCategoryResponse> StockValueByCategory { get; set; } = [];
    public InventoryHealthSummaryResponse Summary { get; set; } = new();
}

public record OverstockAlertResponse
{
    public int TotalOverstockProducts { get; set; }
    public decimal TotalOverstockValue { get; set; }
    public List<OverstockProductResponse> Products { get; set; } = [];
}

public record OverstockProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double CurrentQuantity,
    double RecommendedQuantity,
    decimal ExcessValue,
    decimal CostPrice);

public record ZeroMovementProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    string? SupplierName,
    double CurrentStock,
    decimal StockValue,
    DateTime? LastMovementDate,
    int DaysWithoutMovement);

public record StockValueByCategoryResponse(
    Guid CategoryId,
    string CategoryName,
    int ProductCount,
    decimal TotalStockValue,
    double Percentage);

public record InventoryHealthSummaryResponse
{
    public int TotalProducts { get; set; }
    public int HealthyProducts { get; set; }
    public int OverstockProducts { get; set; }
    public int ZeroMovementProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public decimal OverstockPercentage { get; set; }
    public decimal ZeroMovementPercentage { get; set; }
}
