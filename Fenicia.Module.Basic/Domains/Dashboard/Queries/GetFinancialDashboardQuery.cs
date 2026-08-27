using Fenicia.Module.Basic.Domains.Dashboard.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Dashboard.Queries;

public record GetFinancialDashboardQuery(int Days = 90) : IRequest<FinancialDashboardResponse>;
