namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

public record StockTurnoverResponse(

    Guid ProductId,

    string ProductName,

    string CategoryName,

    double CurrentStock,

    double TotalSold,

    double TurnoverRate,

    string TurnoverClassification);