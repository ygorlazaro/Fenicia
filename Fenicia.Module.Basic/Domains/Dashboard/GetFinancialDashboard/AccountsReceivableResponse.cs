namespace Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

public record AccountsReceivableResponse
{
    public decimal TotalPending { get; set; }
    public int PendingOrdersCount { get; set; }
    public decimal TotalApproved { get; set; }
    public int ApprovedOrdersCount { get; set; }
}
