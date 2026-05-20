using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Commands;

/// <summary>
///     Command to delete a position (soft delete).
/// </summary>
public record DeletePositionCommand(Guid Id) : IRequest;
