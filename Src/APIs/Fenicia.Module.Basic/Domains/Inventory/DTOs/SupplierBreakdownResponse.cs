namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record SupplierBreakdownResponse(

    Guid SupplierId,

    string SupplierName,

    decimal TotalCostValue,

    decimal TotalSalesValue,

    double TotalQuantity);
