namespace Fenicia.Common.DTOs.Basic.Dashboard;

public class GetFinancialDashboardQuery()
{
    public GetFinancialDashboardQuery(int days = 90)
        : this()
    {
        Days = days;
    }

    public int Days { get; set; }
}
