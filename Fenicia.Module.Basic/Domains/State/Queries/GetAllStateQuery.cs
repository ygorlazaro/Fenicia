using Fenicia.Module.Basic.Domains.State.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.State.Queries;

/// <summary>
///     Query record for retrieving all states.
/// </summary>
public record GetAllStateQuery : IRequest<List<GetAllStateResponse>>;
