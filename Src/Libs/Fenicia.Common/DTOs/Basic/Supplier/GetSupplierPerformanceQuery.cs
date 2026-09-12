namespace Fenicia.Common.DTOs.Basic.Supplier;

public record GetSupplierPerformanceQuery(
    int Days = 90,
    int TopLimit = 10);