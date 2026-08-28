namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs;

public record TopMovedProductResponse(

    Guid ProductId,

    string ProductName,

    string CategoryName,

    double TotalMoved,

    decimal TotalValue,

    int MovementCount);
