namespace Fenicia.Module.Basic.Domains.Supplier.GetSupplierPerformance;

public record GetSupplierPerformanceQuery(
    int Days = 90,
    int TopLimit = 10);
