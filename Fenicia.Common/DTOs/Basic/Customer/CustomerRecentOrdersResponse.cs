using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

/// <summary>
/// Response DTO containing recent order details for a customer.
/// </summary>
public class CustomerRecentOrdersResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerRecentOrdersResponse"/> class with the specified recent order details.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="customerName">The name of the customer.</param>
    /// <param name="totalAmount">The total amount of the order.</param>
    /// <param name="saleDate">The date of the sale.</param>
    /// <param name="status">The status of the order.</param>
    /// <param name="totalItems">The total number of items in the order.</param>
    public CustomerRecentOrdersResponse(
        Guid orderId,
        Guid customerId,
        string customerName,
        decimal totalAmount,
        DateTime saleDate,
        string status,
        int totalItems)
        : this()
    {
        OrderId = orderId;
        CustomerId = customerId;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        SaleDate = saleDate;
        Status = status;
        TotalItems = totalItems;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the order.
    /// </summary>
    [Required]
    public Guid OrderId { get; set; }

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
    /// Gets or sets the total amount of the order.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the date of the sale.
    /// </summary>
    [Required]
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the status of the order.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total number of items in the order.
    /// </summary>
    public int TotalItems { get; set; }
}