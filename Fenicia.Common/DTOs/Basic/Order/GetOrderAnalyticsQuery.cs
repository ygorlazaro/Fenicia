namespace Fenicia.Common.DTOs.Basic.Order;

public record GetOrderAnalyticsQuery(int Days = 90, int TopCustomersLimit = 10);