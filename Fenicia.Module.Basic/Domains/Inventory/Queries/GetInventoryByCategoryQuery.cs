namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

/// <summary>
/// Query record for retrieving inventory by category ID.
/// </summary>
public record GetInventoryByCategoryQuery(Guid CategoryId, int Page = 1, int PerPage = 10);
