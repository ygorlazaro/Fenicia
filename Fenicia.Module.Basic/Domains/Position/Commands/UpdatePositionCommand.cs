namespace Fenicia.Module.Basic.Domains.Position.Commands;

/// <summary>
/// Command to update an existing position.
/// </summary>
public record UpdatePositionCommand(Guid Id, string Name);