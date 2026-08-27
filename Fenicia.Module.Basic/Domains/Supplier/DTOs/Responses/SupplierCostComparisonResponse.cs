namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;

public record SupplierCostComparisonResponse(

    string ProductName,

    List<ProductSupplierPriceResponse> Suppliers);