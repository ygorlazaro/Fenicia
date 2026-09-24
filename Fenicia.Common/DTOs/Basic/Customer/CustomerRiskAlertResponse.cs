using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

/// <summary>
/// Response DTO containing customer risk alert details.
/// </summary>
public class CustomerRiskAlertResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerRiskAlertResponse"/> class with the specified risk alert details.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="customerName">The name of the customer.</param>
    /// <param name="previousOrderCount">The previous order count.</param>
    /// <param name="lastOrderDate">The date of the last order.</param>
    /// <param name="daysSinceLastOrder">The number of days since the last order.</param>
    /// <param name="previousTotalSpent">The previous total amount spent.</param>
    /// <param name="riskLevel">The risk level (Low, Medium, High).</param>
    public CustomerRiskAlertResponse(
        Guid customerId,
        string customerName,
        int previousOrderCount,
        DateTime lastOrderDate,
        int daysSinceLastOrder,
        decimal previousTotalSpent,
        string riskLevel)
        : this()
    {
        CustomerId = customerId;
        CustomerName = customerName;
        PreviousOrderCount = previousOrderCount;
        LastOrderDate = lastOrderDate;
        DaysSinceLastOrder = daysSinceLastOrder;
        PreviousTotalSpent = previousTotalSpent;
        RiskLevel = riskLevel;
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
    /// Gets or sets the previous order count.
    /// </summary>
    public int PreviousOrderCount { get; set; }

    /// <summary>
    /// Gets or sets the date of the last order.
    /// </summary>
    [Required]
    public DateTime LastOrderDate { get; set; }

    /// <summary>
    /// Gets or sets the number of days since the last order.
    /// </summary>
    public int DaysSinceLastOrder { get; set; }

    /// <summary>
    /// Gets or sets the previous total amount spent.
    /// </summary>
    public decimal PreviousTotalSpent { get; set; }

    /// <summary>
    /// Gets or sets the risk level.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string RiskLevel { get; set; } = string.Empty;
}