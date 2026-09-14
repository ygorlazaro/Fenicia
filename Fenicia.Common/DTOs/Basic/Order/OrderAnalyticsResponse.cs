namespace Fenicia.Common.DTOs.Basic.Order;

public record OrderAnalyticsResponse
{
    public List<OrderStatusCountResponse> OrdersByStatus { get; set; } = [];

    public List<SalesTrendResponse> SalesTrend { get; set; } = [];

    public List<TopCustomerResponse> TopCustomers { get; set; } = [];

    public AverageOrderValueResponse AverageOrderValue { get; set; } = new();

    public List<CancelledOrderResponse> CancelledOrders { get; set; } = [];
}