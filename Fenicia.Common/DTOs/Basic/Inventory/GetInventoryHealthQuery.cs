namespace Fenicia.Common.DTOs.Basic.Inventory;

public class GetInventoryHealthQuery()
{
    public GetInventoryHealthQuery(int zeroMovementDays = 90)
        : this()
    {
        ZeroMovementDays = zeroMovementDays;
    }

    public int ZeroMovementDays { get; set; }
}
