namespace Fenicia.Module.Basic.Domains.Position.Responses;

/// <summary>
///     Response containing the created position data.
/// </summary>
public record AddPositionResponse(Guid Id, string Name);