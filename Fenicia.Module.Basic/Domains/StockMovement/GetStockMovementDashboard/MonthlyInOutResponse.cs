namespace Fenicia.Module.Basic.Domains.StockMovement.GetStockMovementDashboard;

public record MonthlyInOutResponse(
    string Month,
    double TotalIn,
    double TotalOut,
    decimal TotalInValue,
    decimal TotalOutValue);
