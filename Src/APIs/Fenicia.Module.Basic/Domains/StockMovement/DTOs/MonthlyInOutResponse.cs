namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record MonthlyInOutResponse(

    string Month,

    double TotalIn,

    double TotalOut,

    decimal TotalInValue,

    decimal TotalOutValue);
