using Fenicia.Module.Basic.Domains.Dashboard.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Dashboard.Queries;

/// <summary>
///     Query record for retrieving the financial dashboard.
/// </summary>
public record GetFinancialDashboardQuery(int Days = 90) : IRequest<FinancialDashboardResponse>;
