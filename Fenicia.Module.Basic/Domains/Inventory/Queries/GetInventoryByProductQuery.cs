using Fenicia.Module.Basic.Domains.Inventory.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

public record GetInventoryByProductQuery(Guid ProductId, int Page = 1, int PerPage = 10) : IRequest<InventoryResponse>;
