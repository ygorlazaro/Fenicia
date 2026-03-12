namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

public record MonthlyInOutResponse(
    string Month,
    double TotalIn,
    double TotalOut,
    decimal TotalInValue,
    decimal TotalOutValue);
