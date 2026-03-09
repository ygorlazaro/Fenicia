namespace Fenicia.Module.Basic.Domains.Supplier.GetSupplierPerformance;

public record SupplierCostComparisonResponse(
    string ProductName,
    List<ProductSupplierPriceResponse> Suppliers);
