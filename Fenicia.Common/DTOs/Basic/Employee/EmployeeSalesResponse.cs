using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class EmployeeSalesResponse()
{
    public EmployeeSalesResponse(
        Guid employeeId,
        string employeeName,
        string positionName,
        decimal totalSales,
        int totalOrders,
        decimal averageOrderValue,
        int rank)
        : this()
    {
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        PositionName = positionName;
        TotalSales = totalSales;
        TotalOrders = totalOrders;
        AverageOrderValue = averageOrderValue;
        Rank = rank;
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

    public decimal AverageOrderValue { get; set; }

    public int Rank { get; set; }
}
