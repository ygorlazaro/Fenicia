namespace Fenicia.Module.Basic.Domains.State.Responses;

/// <summary>
/// Response record for a Brazilian state.
/// </summary>
public record GetAllStateResponse(
    /// <summary>
    /// Unique identifier of the state.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Full name of the state.
    /// </summary>
    string Name,
    /// <summary>
    /// Two-letter UF code of the state (e.g., SP, RJ, MG).
    /// </summary>
    string Uf);
