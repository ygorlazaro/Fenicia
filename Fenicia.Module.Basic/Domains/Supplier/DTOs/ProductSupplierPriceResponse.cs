namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record ProductSupplierPriceResponse(

    Guid SupplierId,

    string SupplierName,

    decimal CostPrice,

    decimal SalesPrice,

    decimal ProfitMargin);
