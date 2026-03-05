namespace Fenicia.Module.Basic.Domains.Customer.GetCustomerInsights;

public record GetCustomerInsightsQuery(
    int Days = 90,
    int TopLimit = 10,
    int RiskThresholdDays = 60);
