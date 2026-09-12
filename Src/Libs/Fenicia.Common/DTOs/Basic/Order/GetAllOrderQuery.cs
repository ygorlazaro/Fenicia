namespace Fenicia.Common.DTOs.Basic.Order;

public record GetAllOrderQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);