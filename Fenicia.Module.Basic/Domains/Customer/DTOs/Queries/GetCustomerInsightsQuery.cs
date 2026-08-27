using Fenicia.Module.Basic.Domains.Customer.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Customer.DTOs.Queries;

public record GetCustomerInsightsQuery(int Days = 90, int TopLimit = 10, int RiskThresholdDays = 60);
