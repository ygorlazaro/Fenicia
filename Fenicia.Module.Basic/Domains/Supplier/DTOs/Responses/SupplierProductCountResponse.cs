namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;

public record SupplierProductCountResponse(

    Guid SupplierId,

    string SupplierName,

    int ProductCount,

    decimal TotalStockValue,

    decimal TotalRevenue);