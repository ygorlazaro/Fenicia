namespace Fenicia.Module.Basic.Domains.Position.Responses;

/// <summary>
///     Response containing position data for list display.
/// </summary>
public record GetAllPositionResponse(Guid Id, string Name);