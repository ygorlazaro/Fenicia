namespace Fenicia.Module.Basic.Domains.Employee.GetEmployeePerformance;

public record EmployeePerformanceResponse
{
    public EmployeePerformanceSummaryResponse Summary { get; set; } = new();
    public List<EmployeeSalesResponse> SalesByEmployee { get; set; } = [];
    public List<EmployeeOrderCountResponse> OrdersByEmployee { get; set; } = [];
    public List<TopPerformerResponse> TopPerformers { get; set; } = [];
}

public record EmployeePerformanceSummaryResponse
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageSalesPerEmployee { get; set; }
    public decimal AverageOrdersPerEmployee { get; set; }
}

public record EmployeeSalesResponse(
    Guid EmployeeId,
    string EmployeeName,
    string PositionName,
    decimal TotalSales,
    int TotalOrders,
    decimal AverageOrderValue,
    int Rank);

public record EmployeeOrderCountResponse(
    Guid EmployeeId,
    string EmployeeName,
    string PositionName,
    int OrderCount,
    decimal TotalValue,
    DateTime FirstOrderDate,
    DateTime LastOrderDate);

public record TopPerformerResponse(
    Guid EmployeeId,
    string EmployeeName,
    string PositionName,
    decimal TotalSales,
    int TotalOrders,
    string PerformanceLevel);
