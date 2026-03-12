namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

public record StockValueByCategoryResponse(
    Guid CategoryId,
    string CategoryName,
    int ProductCount,
    decimal TotalStockValue,
    double Percentage);
