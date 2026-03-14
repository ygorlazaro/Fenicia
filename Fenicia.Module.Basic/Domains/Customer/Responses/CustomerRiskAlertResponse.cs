namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
///     Response model for customer risk alerts.
///     Identifies customers who have been inactive for an extended period.
/// </summary>
public record CustomerRiskAlertResponse(Guid CustomerId, string CustomerName, int PreviousOrderCount, DateTime LastOrderDate, int DaysSinceLastOrder, decimal PreviousTotalSpent, string RiskLevel);