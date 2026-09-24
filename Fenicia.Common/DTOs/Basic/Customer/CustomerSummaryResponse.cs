using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

/// <summary>
/// Response DTO containing customer summary metrics.
/// </summary>
public class CustomerSummaryResponse
{
    /// <summary>
    /// Gets or sets the total number of customers.
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// Gets or sets the total number of orders.
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Gets or sets the total revenue.
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Gets or sets the average order value.
    /// </summary>
    public decimal AverageOrderValue { get; set; }
}