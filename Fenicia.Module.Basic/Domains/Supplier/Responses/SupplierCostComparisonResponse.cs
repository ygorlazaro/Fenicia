namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

public record SupplierCostComparisonResponse(
    string ProductName,
    List<ProductSupplierPriceResponse> Suppliers);
