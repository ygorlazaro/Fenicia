namespace Fenicia.Module.Basic.Domains.Employee.Responses;

public record EmployeePerformanceResponse
{
    public EmployeePerformanceSummaryResponse Summary { get; set; } = new();
    public List<EmployeeSalesResponse> SalesByEmployee { get; set; } = [];
    public List<EmployeeOrderCountResponse> OrdersByEmployee { get; set; } = [];
    public List<TopPerformerResponse> TopPerformers { get; set; } = [];
}
