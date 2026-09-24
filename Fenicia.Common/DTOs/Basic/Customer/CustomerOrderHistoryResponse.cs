using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

/// <summary>
/// Response DTO containing customer order history details.
/// </summary>
public class CustomerOrderHistoryResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerOrderHistoryResponse"/> class with the specified order history details.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="customerName">The name of the customer.</param>
    /// <param name="orderCount">The total number of orders.</param>
    /// <param name="totalSpent">The total amount spent by the customer.</param>
    /// <param name="totalItems">The total number of items ordered.</param>
    /// <param name="firstOrderDate">The date of the first order.</param>
    /// <param name="lastOrderDate">The date of the last order.</param>
    /// <param name="averageOrderValue">The average order value.</param>
    public CustomerOrderHistoryResponse(
        Guid customerId,
        string customerName,
        int orderCount,
        decimal totalSpent,
        int totalItems,
        DateTime firstOrderDate,
        DateTime lastOrderDate,
        decimal averageOrderValue)
        : this()
    {
        CustomerId = customerId;
        CustomerName = customerName;
        OrderCount = orderCount;
        TotalSpent = totalSpent;
        TotalItems = totalItems;
        FirstOrderDate = firstOrderDate;
        LastOrderDate = lastOrderDate;
        AverageOrderValue = averageOrderValue;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of orders.
    /// </summary>
    public int OrderCount { get; set; }

    /// <summary>
    /// Gets or sets the total amount spent by the customer.
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// Gets or sets the total number of items ordered.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the date of the first order.
    /// </summary>
    [Required]
    public DateTime FirstOrderDate { get; set; }

    /// <summary>
    /// Gets or sets the date of the last order.
    /// </summary>
    [Required]
    public DateTime LastOrderDate { get; set; }

    /// <summary>
    /// Gets or sets the average order value.
    /// </summary>
    public decimal AverageOrderValue { get; set; }
}