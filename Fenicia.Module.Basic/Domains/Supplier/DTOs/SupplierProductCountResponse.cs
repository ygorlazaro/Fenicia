namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record SupplierProductCountResponse(

    Guid SupplierId,

    string SupplierName,

    int ProductCount,

    decimal TotalStockValue,

    decimal TotalRevenue);
