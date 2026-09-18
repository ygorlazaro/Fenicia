using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public class CustomerRiskAlertResponse()
{
    public CustomerRiskAlertResponse(Guid customerId, string customerName, int previousOrderCount, DateTime lastOrderDate, int daysSinceLastOrder, decimal previousTotalSpent, string riskLevel)
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

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CustomerName { get; set; } = string.Empty;

    public int PreviousOrderCount { get; set; }

    [Required]
    public DateTime LastOrderDate { get; set; }

    public int DaysSinceLastOrder { get; set; }

    public decimal PreviousTotalSpent { get; set; }

    [Required]
    [MaxLength(200)]
    public string RiskLevel { get; set; } = string.Empty;
}
