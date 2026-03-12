namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

public record GetInventoryByProductQuery(Guid ProductId, int Page = 1, int PerPage = 10);