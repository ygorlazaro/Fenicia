namespace Fenicia.Module.Basic.Domains.State.DTOs.Responses;

public record GetAllStateResponse(
    Guid Id,
    string Name,
    string Uf);
