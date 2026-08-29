namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record CategoryBreakdownResponse(

    Guid CategoryId,

    string CategoryName,

    decimal TotalCostValue,

    decimal TotalSalesValue,

    double TotalQuantity);
