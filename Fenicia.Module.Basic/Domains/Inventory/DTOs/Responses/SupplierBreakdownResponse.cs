namespace Fenicia.Module.Basic.Domains.Inventory.DTOs.Responses;

public record SupplierBreakdownResponse(

    Guid SupplierId,

    string SupplierName,

    decimal TotalCostValue,

    decimal TotalSalesValue,

    double TotalQuantity);