namespace Fenicia.Common.DTOs.Basic.Supplier;

public class GetSupplierPerformanceQuery()
{
    public GetSupplierPerformanceQuery(int days = 90, int topLimit = 10)
        : this()
    {
        Days = days;
        TopLimit = topLimit;
    }

    public int Days { get; set; } = 90;

    public int TopLimit { get; set; } = 10;
}
