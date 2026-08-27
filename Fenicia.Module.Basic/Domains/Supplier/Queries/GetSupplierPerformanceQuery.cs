using MediatR;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Queries;

public record GetSupplierPerformanceQuery(

    int Days = 90,

    int TopLimit = 10) : IRequest<SupplierPerformanceResponse>;