using Fenicia.Module.Basic.Domains.Product.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.Product.DTOs.Queries;

public record GetProductPerformanceQuery(

    int Days = 90,

    int TopLimit = 10);