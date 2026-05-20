using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Commands;

/// <summary>
///     Command to create a new position.
/// </summary>
public record AddPositionCommand(Guid Id, string Name) : IRequest<AddPositionResponse>;
