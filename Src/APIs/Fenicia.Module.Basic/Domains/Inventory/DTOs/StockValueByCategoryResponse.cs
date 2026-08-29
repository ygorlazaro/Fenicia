namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record StockValueByCategoryResponse(

    Guid CategoryId,

    string CategoryName,

    int ProductCount,

    decimal TotalStockValue,

    double Percentage);
