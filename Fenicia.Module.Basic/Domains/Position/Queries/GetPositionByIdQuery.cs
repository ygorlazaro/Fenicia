using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Queries;

public record GetPositionByIdQuery(Guid Id) : IRequest<GetPositionByIdResponse?>;
