using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Commands;

public record DeletePositionCommand(Guid Id) : IRequest;
