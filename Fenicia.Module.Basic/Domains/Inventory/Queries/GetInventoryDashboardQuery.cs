using Fenicia.Module.Basic.Domains.Inventory.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Inventory.Queries;

/// <summary>
///     Query record for retrieving inventory dashboard metrics.
/// </summary>
public record GetInventoryDashboardQuery : IRequest<InventoryDashboardResponse>;
