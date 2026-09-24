using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Response DTO containing top performer data.
/// </summary>
public class TopPerformerResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TopPerformerResponse"/> class with the specified top performer data.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="employeeName">The name of the employee.</param>
    /// <param name="positionName">The name of the position.</param>
    /// <param name="totalSales">The total sales amount.</param>
    /// <param name="totalOrders">The total number of orders.</param>
    /// <param name="performanceLevel">The performance level (e.g., Excellent, Very Good, Good, Standard).</param>
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
    /// Gets or sets the performance level.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string PerformanceLevel { get; set; } = string.Empty;
}