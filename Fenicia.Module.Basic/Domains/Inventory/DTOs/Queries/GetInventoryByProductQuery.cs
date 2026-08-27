using Fenicia.Module.Basic.Domains.Inventory.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Inventory.DTOs.Queries;

public record GetInventoryByProductQuery(Guid ProductId, int Page = 1, int PerPage = 10);
