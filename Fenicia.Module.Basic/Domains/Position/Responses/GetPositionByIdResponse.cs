namespace Fenicia.Module.Basic.Domains.Position.Responses;

/// <summary>
///     Response containing detailed position data.
/// </summary>
public record GetPositionByIdResponse(Guid Id, string Name);