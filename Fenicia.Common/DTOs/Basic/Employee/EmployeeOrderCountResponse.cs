using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Response DTO containing employee order count data.
/// </summary>
public class EmployeeOrderCountResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeOrderCountResponse"/> class with the specified order count data.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="employeeName">The name of the employee.</param>
    /// <param name="positionName">The name of the position.</param>
    /// <param name="orderCount">The number of orders.</param>
    /// <param name="totalValue">The total value of orders.</param>
    /// <param name="firstOrderDate">The date of the first order.</param>
    /// <param name="lastOrderDate">The date of the last order.</param>
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

    /// <summary>
    /// Gets or sets the unique identifier of the employee.
    /// </summary>
    [Required]
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the employee.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the position.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string PositionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of orders.
    /// </summary>
    public int OrderCount { get; set; }

    /// <summary>
    /// Gets or sets the total value of orders.
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Gets or sets the date of the first order.
    /// </summary>
    [Required]
    public DateTime FirstOrderDate { get; set; }

    /// <summary>
    /// Gets or sets the date of the last order.
    /// </summary>
    [Required]
    public DateTime LastOrderDate { get; set; }
}