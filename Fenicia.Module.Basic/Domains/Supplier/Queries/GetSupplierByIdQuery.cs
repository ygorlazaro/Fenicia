using MediatR;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Queries;

/// <summary>
///     Query record for retrieving a supplier by its ID.
/// </summary>
public record GetSupplierByIdQuery(
    /// <summary>
    /// Unique identifier of the supplier.
    /// </summary>
    Guid Id) : IRequest<GetSupplierByIdResponse?>;