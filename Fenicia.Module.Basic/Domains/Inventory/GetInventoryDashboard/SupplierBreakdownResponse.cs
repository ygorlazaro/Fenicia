namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryDashboard;

public record SupplierBreakdownResponse(
    Guid SupplierId,
    string SupplierName,
    decimal TotalCostValue,
    decimal TotalSalesValue,
    double TotalQuantity);
