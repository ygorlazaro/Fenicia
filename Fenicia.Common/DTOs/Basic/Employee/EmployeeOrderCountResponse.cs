using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class EmployeeOrderCountResponse()
{
    public EmployeeOrderCountResponse(
        Guid employeeId,
        string employeeName,
        string positionName,
        int orderCount,
        decimal totalValue,
        DateTime firstOrderDate,
        DateTime lastOrderDate)
        : this()
    {
        EmployeeId = employeeId;
        EmployeeName = employeeName;
        PositionName = positionName;
        OrderCount = orderCount;
        TotalValue = totalValue;
        FirstOrderDate = firstOrderDate;
        LastOrderDate = lastOrderDate;
    }

    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    [MaxLength(200)]
    public string EmployeeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string PositionName { get; set; } = string.Empty;

    public int OrderCount { get; set; }

    public decimal TotalValue { get; set; }

    [Required]
    public DateTime FirstOrderDate { get; set; }

    [Required]
    public DateTime LastOrderDate { get; set; }
}
