namespace Fenicia.Common.DTOs.Basic.Employee;

public class EmployeePerformanceResponse
{
    public EmployeePerformanceSummaryResponse Summary { get; set; } = new();

    public List<EmployeeSalesResponse> SalesByEmployee { get; set; } = [];

    public List<EmployeeOrderCountResponse> OrdersByEmployee { get; set; } = [];

    public List<TopPerformerResponse> TopPerformers { get; set; } = [];
}
