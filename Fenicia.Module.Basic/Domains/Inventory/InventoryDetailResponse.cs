namespace Fenicia.Module.Basic.Domains.Inventory;

public record InventoryDetailResponse(
    Guid Id,
    string Name,
    double Quantity,
    decimal? CostPrice,
    decimal SalesPrice,
    Guid CategoryId,
    string CategoryName);