namespace Fenicia.Module.Basic.Domains.Dashboard.Responses;

/// <summary>
/// Response model for accounts receivable summary.
/// Shows pending and approved order amounts.
/// </summary>
public record AccountsReceivableResponse
{
    /// <summary>Total amount of pending orders.</summary>
    public decimal TotalPending { get; set; }
    /// <summary>Number of pending orders.</summary>
    public int PendingOrdersCount { get; set; }
    /// <summary>Total amount of approved orders.</summary>
    public decimal TotalApproved { get; set; }
    /// <summary>Number of approved orders.</summary>
    public int ApprovedOrdersCount { get; set; }
}
