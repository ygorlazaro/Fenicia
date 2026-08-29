namespace Fenicia.Module.Basic.Domains.State.DTOs;

public record GetAllStateResponse(
    Guid Id,
    string Name,
    string Uf);
