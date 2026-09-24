using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Request DTO for creating or updating an order.
/// </summary>
public class OrderRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRequest"/> class with the specified order details.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="saleDate">The sale date.</param>
    /// <param name="status">The order status.</param>
    /// <param name="details">The order details.</param>
    /// <param name="paymentMethod">The payment method.</param>
    /// <param name="employeeId">The unique identifier of the employee (optional).</param>
    /// <param name="notes">Additional notes (optional).</param>
    /// <param name="discountAmount">The discount amount (optional).</param>
    public OrderRequest(
        Guid userId,
        Guid customerId,
        DateTime saleDate,
        EnumOrderStatus status,
        List<OrderDetailRequest> details,
        EnumPaymentMethod paymentMethod,
        Guid? employeeId = null,
        string? notes = null,
        decimal discountAmount = 0)
        : this()
    {
        UserId = userId;
        CustomerId = customerId;
        SaleDate = saleDate;
        Status = status;
        Details = details;
        PaymentMethod = paymentMethod;
        EmployeeId = employeeId;
        Notes = notes;
        DiscountAmount = discountAmount;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the sale date.
    /// </summary>
    [Required]
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    [Required]
    public EnumOrderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the order details.
    /// </summary>
    public List<OrderDetailRequest> Details { get; set; } = [];

    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    [Required]
    public EnumPaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the employee.
    /// </summary>
    public Guid? EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets additional notes.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the discount amount.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }
}