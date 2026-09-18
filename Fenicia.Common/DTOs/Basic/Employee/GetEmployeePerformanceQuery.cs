namespace Fenicia.Common.DTOs.Basic.Employee;

public class GetEmployeePerformanceQuery()
{
    public GetEmployeePerformanceQuery(int days = 90, int topLimit = 10)
        : this()
    {
        Days = days;
        TopLimit = topLimit;
    }

    public int Days { get; set; }

    public int TopLimit { get; set; }
}
