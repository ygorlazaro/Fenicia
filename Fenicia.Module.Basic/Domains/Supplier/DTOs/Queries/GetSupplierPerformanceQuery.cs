using Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Queries;

public record GetSupplierPerformanceQuery(

    int Days = 90,

    int TopLimit = 10);