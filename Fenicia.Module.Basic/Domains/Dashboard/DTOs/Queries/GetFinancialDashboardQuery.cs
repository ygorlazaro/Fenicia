using Fenicia.Module.Basic.Domains.Dashboard.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs.Queries;

public record GetFinancialDashboardQuery(int Days = 90);
