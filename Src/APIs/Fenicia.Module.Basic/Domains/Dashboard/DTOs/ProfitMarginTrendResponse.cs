namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs;

public record ProfitMarginTrendResponse(

    string Period,

    DateTime Date,

    decimal MarginPercentage,

    string Trend);
