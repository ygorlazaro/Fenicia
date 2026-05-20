using MediatR;
using Fenicia.Module.Basic.Domains.Product.Responses;

namespace Fenicia.Module.Basic.Domains.Product.Queries;

/// <summary>
///     Query record for retrieving products by category ID with pagination.
/// </summary>
public record GetProductsByCategoryIdQuery(
    /// <summary>
    /// Category ID to filter products.
    /// </summary>
    Guid CategoryId,
    /// <summary>
    /// Page number for pagination.
    /// </summary>
    int Page = 1,
    /// <summary>
    /// Number of items per page.
    /// </summary>
    int PerPage = 10) : IRequest<List<GetProductsByCategoryIdResponse>>;