using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Commands;

/// <summary>
///     Command to update an existing position.
/// </summary>
public record UpdatePositionCommand(Guid Id, string Name) : IRequest<UpdatePositionResponse?>;
