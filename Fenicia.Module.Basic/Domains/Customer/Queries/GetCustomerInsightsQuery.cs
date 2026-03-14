namespace Fenicia.Module.Basic.Domains.Customer.Queries;

/// <summary>
/// Query record for generating customer insights and analytics.
/// </summary>
public record GetCustomerInsightsQuery(
    int Days = 90,
    int TopLimit = 10,
    int RiskThresholdDays = 60);
