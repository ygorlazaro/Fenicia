using MediatR;
using Fenicia.Module.Basic.Domains.Product.Responses;

namespace Fenicia.Module.Basic.Domains.Product.Queries;

public record GetProductPerformanceQuery(

    int Days = 90,

    int TopLimit = 10) : IRequest<ProductPerformanceResponse>;