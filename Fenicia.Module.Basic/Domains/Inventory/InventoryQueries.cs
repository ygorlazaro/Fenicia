namespace Fenicia.Module.Basic.Domains.Inventory;

public record GetInventoryQuery(int Page = 1, int PerPage = 10);
public record GetInventoryByProductQuery(Guid ProductId, int Page = 1, int PerPage = 10);
public record GetInventoryByCategoryQuery(Guid CategoryId, int Page = 1, int PerPage = 10);
