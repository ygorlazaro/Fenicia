namespace Fenicia.Module.Basic.Domains.StockMovement.DTOs.Responses;

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