using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Queries;

/// <summary>
///     Query to retrieve a position by its unique identifier.
/// </summary>
public record GetPositionByIdQuery(Guid Id) : IRequest<GetPositionByIdResponse?>;
