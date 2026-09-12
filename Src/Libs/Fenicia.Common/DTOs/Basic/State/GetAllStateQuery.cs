namespace Fenicia.Common.DTOs.Basic.State;

public record GetAllStateQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);