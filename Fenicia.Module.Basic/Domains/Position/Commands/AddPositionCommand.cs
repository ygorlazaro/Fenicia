using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Commands;

public record AddPositionCommand(Guid Id, string Name) : IRequest<AddPositionResponse>;
