namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
/// Response model containing aggregate statistics about customers.
/// Provides business intelligence summary metrics.
/// </summary>
public record CustomerSummaryResponse
{
    /// <summary>Total number of customers in the system.</summary>
    public int TotalCustomers { get; set; }
    /// <summary>Total number of orders placed by all customers.</summary>
    public int TotalOrders { get; set; }
    /// <summary>Total revenue generated from all customer orders.</summary>
    public decimal TotalRevenue { get; set; }
    /// <summary>Average value per order.</summary>
    public decimal AverageOrderValue { get; set; }
    /// <summary>Average total spending per customer over their lifetime.</summary>
    public decimal AverageCustomerLifetimeValue { get; set; }
}
