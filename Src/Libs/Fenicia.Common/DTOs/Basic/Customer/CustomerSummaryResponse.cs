namespace Fenicia.Common.DTOs.Basic.Customer;

public record CustomerSummaryResponse
{
    public int TotalCustomers { get; set; }

    public int TotalOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal AverageOrderValue { get; set; }
}