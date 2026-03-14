namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

/// <summary>
/// Query record for retrieving inventory by product ID.
/// </summary>
public record GetInventoryByProductQuery(Guid ProductId, int Page = 1, int PerPage = 10);