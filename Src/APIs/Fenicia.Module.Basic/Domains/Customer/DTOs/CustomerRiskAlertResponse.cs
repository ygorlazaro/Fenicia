namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record CustomerRiskAlertResponse(Guid CustomerId, string CustomerName, int PreviousOrderCount, DateTime LastOrderDate, int DaysSinceLastOrder, decimal PreviousTotalSpent, string RiskLevel);
