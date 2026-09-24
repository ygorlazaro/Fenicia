namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Response DTO containing accounts receivable data.
/// </summary>
public class AccountsReceivableResponse
{
    /// <summary>
    /// Gets or sets the total pending amount.
    /// </summary>
    public decimal TotalPending { get; set; }

    /// <summary>
    /// Gets or sets the pending orders count.
    /// </summary>
    public int PendingOrdersCount { get; set; }

    /// <summary>
    /// Gets or sets the total approved amount.
    /// </summary>
    public decimal TotalApproved { get; set; }

    /// <summary>
    /// Gets or sets the approved orders count.
    /// </summary>
    public int ApprovedOrdersCount { get; set; }
}