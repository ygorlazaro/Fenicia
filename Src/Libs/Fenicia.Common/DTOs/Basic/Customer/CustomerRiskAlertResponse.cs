using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record CustomerRiskAlertResponse(
    [Required] Guid CustomerId,
    [Required] [MaxLength(200)] string CustomerName,
    int PreviousOrderCount,
    [Required] DateTime LastOrderDate,
    int DaysSinceLastOrder,
    decimal PreviousTotalSpent,
    [Required] [MaxLength(200)] string RiskLevel);