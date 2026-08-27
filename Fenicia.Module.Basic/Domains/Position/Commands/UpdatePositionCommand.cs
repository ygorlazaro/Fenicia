using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Commands;

public record UpdatePositionCommand(Guid Id, string Name) : IRequest<UpdatePositionResponse?>;
