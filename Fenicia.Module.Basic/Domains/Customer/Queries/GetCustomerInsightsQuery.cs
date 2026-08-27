using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Queries;

public record GetCustomerInsightsQuery(int Days = 90, int TopLimit = 10, int RiskThresholdDays = 60) : IRequest<CustomerInsightsResponse>;
