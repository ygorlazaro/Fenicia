namespace Fenicia.Module.Basic.Domains.Customer.GetCustomerInsights;

public record CustomerSummaryResponse
{
    public int TotalCustomers { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal AverageCustomerLifetimeValue { get; set; }
}
