namespace Fenicia.Module.Basic.Domains.StockMovement.GetStockMovementDashboard;

public record StockMovementDashboardResponse
{
    public List<StockMovementHistoryResponse> History { get; set; } = [];
    public List<MonthlyInOutResponse> MonthlyInOut { get; set; } = [];
    public List<TopMovedProductResponse> TopMovedProducts { get; set; } = [];
    public List<StockTurnoverResponse> TurnoverRates { get; set; } = [];
}

public record StockMovementHistoryResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    double Quantity,
    DateTime Date,
    decimal Price,
    string Type,
    string? Reason,
    string? CustomerName,
    string? SupplierName);

public record MonthlyInOutResponse(
    string Month,
    double TotalIn,
    double TotalOut,
    decimal TotalInValue,
    decimal TotalOutValue);

public record TopMovedProductResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double TotalMoved,
    decimal TotalValue,
    int MovementCount);

public record StockTurnoverResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    double CurrentStock,
    double TotalSold,
    double TurnoverRate,
    string TurnoverClassification);
