using Fenicia.Module.Basic.Domains.Order.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Order.Queries;

/// <summary>
///     Query to retrieve a specific order by ID.
/// </summary>
public record GetOrderByIdQuery(Guid Id) : IRequest<GetOrderByIdResponse?>;
