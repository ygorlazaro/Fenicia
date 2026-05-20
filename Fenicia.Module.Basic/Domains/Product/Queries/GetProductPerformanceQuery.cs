using MediatR;
using Fenicia.Module.Basic.Domains.Product.Responses;

namespace Fenicia.Module.Basic.Domains.Product.Queries;

/// <summary>
///     Query record for retrieving product performance metrics.
/// </summary>
public record GetProductPerformanceQuery(
    /// <summary>
    /// Number of days to analyze for performance metrics.
    /// </summary>
    int Days = 90,
    /// <summary>
    /// Number of top/bottom products to return.
    /// </summary>
    int TopLimit = 10) : IRequest<ProductPerformanceResponse>;