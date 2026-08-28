namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record GetOrderAnalyticsQuery(int Days = 90, int TopCustomersLimit = 10);
