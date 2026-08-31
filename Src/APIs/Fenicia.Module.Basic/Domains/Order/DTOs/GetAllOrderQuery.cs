namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record GetAllOrderQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);
