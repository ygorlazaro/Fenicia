using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

/// <summary>
/// Response DTO for employee data source (dropdowns, lookups).
/// </summary>
public class EmployeeDataSourceResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeDataSourceResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the employee.</param>
    /// <param name="name">The name of the employee.</param>
    public EmployeeDataSourceResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the employee.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the employee.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}