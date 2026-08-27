using Fenicia.Module.Basic.Domains.Inventory.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Inventory.DTOs.Queries;

public record GetInventoryByCategoryQuery(Guid CategoryId, int Page = 1, int PerPage = 10);
