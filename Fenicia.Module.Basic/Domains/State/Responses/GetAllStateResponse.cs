namespace Fenicia.Module.Basic.Domains.State.Responses;

/// <summary>
///     Response record for a Brazilian state.
/// </summary>
public record GetAllStateResponse(
    Guid Id,
    string Name,
    string Uf);
