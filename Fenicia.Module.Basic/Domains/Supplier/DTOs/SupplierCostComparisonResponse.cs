namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record SupplierCostComparisonResponse(

    string ProductName,

    List<ProductSupplierPriceResponse> Suppliers);
