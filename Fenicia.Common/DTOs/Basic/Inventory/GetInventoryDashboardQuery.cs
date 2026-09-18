namespace Fenicia.Common.DTOs.Basic.Inventory;

public class GetInventoryDashboardQuery()
{
    public GetInventoryDashboardQuery(int days = 90)
        : this()
    {
        Days = days;
    }

    public int Days { get; set; }
}
