namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryDashboard;

public record InventoryDashboardItemResponse(
    Guid Id,
    string Name,
    double Quantity,
    decimal? CostPrice,
    decimal SalesPrice,
    Guid CategoryId,
    string CategoryName);
