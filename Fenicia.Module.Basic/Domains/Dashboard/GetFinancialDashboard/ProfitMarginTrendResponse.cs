namespace Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

public record ProfitMarginTrendResponse(
    string Period,
    DateTime Date,
    decimal MarginPercentage,
    string Trend);
