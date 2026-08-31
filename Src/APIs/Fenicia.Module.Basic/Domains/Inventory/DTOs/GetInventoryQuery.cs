namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record GetInventoryQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);
