namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

/// <summary>
/// Query record for retrieving inventory with pagination.
/// </summary>
public record GetInventoryQuery(int Page = 1, int PerPage = 10);
