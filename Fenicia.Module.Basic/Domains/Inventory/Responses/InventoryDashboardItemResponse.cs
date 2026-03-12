namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

public record InventoryDashboardItemResponse(
    Guid Id,
    string Name,
    double Quantity,
    decimal? CostPrice,
    decimal SalesPrice,
    Guid CategoryId,
    string CategoryName);
