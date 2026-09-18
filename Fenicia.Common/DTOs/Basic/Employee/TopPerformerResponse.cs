using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class TopPerformerResponse()
{
    public TopPerformerResponse(
        Guid employeeId,
        string employeeName,
        string positionName,
        decimal totalSales,
        int totalOrders,
        string performanceLevel)
        : this()
    {
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        PositionName = positionName;
        TotalSales = totalSales;
        TotalOrders = totalOrders;
        PerformanceLevel = performanceLevel;
    }

    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    [MaxLength(200)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string PositionName { get; set; } = string.Empty;

    public decimal TotalSales { get; set; }

    public int TotalOrders { get; set; }

    [Required]
    [MaxLength(200)]
    public string PerformanceLevel { get; set; } = string.Empty;
}
