namespace Fenicia.Common.DTOs.Basic.Customer;

public record GetCustomerInsightsQuery(int Days = 90, int TopLimit = 10, int RiskThresholdDays = 60);