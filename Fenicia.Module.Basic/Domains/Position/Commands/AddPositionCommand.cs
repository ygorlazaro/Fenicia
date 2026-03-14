namespace Fenicia.Module.Basic.Domains.Position.Commands;

/// <summary>
/// Command to create a new position.
/// </summary>
public record AddPositionCommand(Guid Id, string Name);
