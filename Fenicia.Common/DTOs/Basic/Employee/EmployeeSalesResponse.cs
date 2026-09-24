using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Response DTO containing employee sales data.
/// </summary>
public class EmployeeSalesResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeSalesResponse"/> class with the specified sales data.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="employeeName">The name of the employee.</param>
    /// <param name="positionName">The name of the position.</param>
    /// <param name="totalSales">The total sales amount.</param>
    /// <param name="totalOrders">The total number of orders.</param>
    /// <param name="averageOrderValue">The average order value.</param>
    /// <param name="rank">The rank of the employee.</param>
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
    /// Gets or sets the total sales amount.
    /// </summary>
    public decimal TotalSales { get; set; }

    /// <summary>
    /// Gets or sets the total number of orders.
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Gets or sets the average order value.
    /// </summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>
    /// Gets or sets the rank of the employee.
    /// </summary>
    public int Rank { get; set; }
}