namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

public record SupplierProductCountResponse(

    Guid SupplierId,

    string SupplierName,

    int ProductCount,

    decimal TotalStockValue,

    decimal TotalRevenue);