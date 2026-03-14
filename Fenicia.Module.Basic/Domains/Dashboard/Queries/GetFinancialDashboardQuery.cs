namespace Fenicia.Module.Basic.Domains.Dashboard.Queries;

/// <summary>
///     Query record for retrieving the financial dashboard.
/// </summary>
public record GetFinancialDashboardQuery(int Days = 90);