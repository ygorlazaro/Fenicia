namespace Fenicia.Module.Basic.Domains.State.DTOs;

public record GetAllStateQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);