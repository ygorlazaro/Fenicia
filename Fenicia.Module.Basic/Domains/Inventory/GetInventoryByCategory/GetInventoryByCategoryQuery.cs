namespace Fenicia.Module.Basic.Domains.Inventory.GetInventoryByCategory;

public record GetInventoryByCategoryQuery(Guid CategoryId, int Page = 1, int PerPage = 10);