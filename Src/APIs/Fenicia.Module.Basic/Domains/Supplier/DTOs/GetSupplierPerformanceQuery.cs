namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record GetSupplierPerformanceQuery(

    int Days = 90,

    int TopLimit = 10);
