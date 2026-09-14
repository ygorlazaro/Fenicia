namespace Fenicia.Common.DTOs.Basic.Dashboard;

public record AccountsReceivableResponse
{
    public decimal TotalPending { get; set; }

    public int PendingOrdersCount { get; set; }

    public decimal TotalApproved { get; set; }

    public int ApprovedOrdersCount { get; set; }
}