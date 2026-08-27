using Fenicia.Module.Basic.Domains.State.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.State.Queries;

public record GetAllStateQuery : IRequest<List<GetAllStateResponse>>;
