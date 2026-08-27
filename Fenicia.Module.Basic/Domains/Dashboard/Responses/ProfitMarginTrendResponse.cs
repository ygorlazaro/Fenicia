namespace Fenicia.Module.Basic.Domains.Dashboard.Responses;

public record ProfitMarginTrendResponse(

    string Period,

    DateTime Date,

    decimal MarginPercentage,

    string Trend);