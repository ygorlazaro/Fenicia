namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs.Responses;

public record ProfitMarginTrendResponse(

    string Period,

    DateTime Date,

    decimal MarginPercentage,

    string Trend);