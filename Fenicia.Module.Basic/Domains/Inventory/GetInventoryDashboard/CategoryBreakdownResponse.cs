namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryDashboard;

public record CategoryBreakdownResponse(
    Guid CategoryId,
    string CategoryName,
    decimal TotalCostValue,
    decimal TotalSalesValue,
    double TotalQuantity);
