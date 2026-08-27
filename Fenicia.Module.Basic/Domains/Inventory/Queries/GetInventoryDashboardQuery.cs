using Fenicia.Module.Basic.Domains.Inventory.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

public record GetInventoryDashboardQuery : IRequest<InventoryDashboardResponse>;
