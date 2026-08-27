using Fenicia.Module.Basic.Domains.Order.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Order.DTOs.Queries;

public record GetOrderAnalyticsQuery(int Days = 90, int TopCustomersLimit = 10);
