using MediatR;
using Fenicia.Module.Basic.Domains.Product.Responses;

namespace Fenicia.Module.Basic.Domains.Product.Queries;

/// <summary>
///     Query record for retrieving a product by its ID.
/// </summary>
public record GetProductByIdQuery(
    /// <summary>
    /// Unique identifier of the product.
    /// </summary>
    Guid Id) : IRequest<GetProductByIdResponse?>;