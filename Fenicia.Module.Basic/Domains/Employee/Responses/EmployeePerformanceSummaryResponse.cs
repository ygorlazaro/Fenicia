namespace Fenicia.Module.Basic.Domains.Employee.Responses;

public record EmployeePerformanceSummaryResponse
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageSalesPerEmployee { get; set; }
    public decimal AverageOrdersPerEmployee { get; set; }
}
