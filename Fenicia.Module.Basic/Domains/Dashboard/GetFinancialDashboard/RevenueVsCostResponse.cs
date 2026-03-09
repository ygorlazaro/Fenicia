namespace Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

public record RevenueVsCostResponse(
    string Period,
    DateTime Date,
    decimal Revenue,
    decimal Cost,
    decimal Profit);
